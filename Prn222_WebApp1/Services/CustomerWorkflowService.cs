using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Domain;
using Repositories;
using EventEntity = Domain.Event;

namespace Services
{
    public class CustomerWorkflowService : ICustomerWorkflowService
    {
        public const int BookingStatusPending = 0;
        public const int BookingStatusPaid = 2;
        public const int TransactionStatusPending = 0;
        public const int TransactionStatusConfirmed = 1;

        private const string QrSecret = "DemoMVC2TicketQrSecret";
        private static readonly Regex RedeemedPointsRegex = new(@"Redeemed (\d+) point", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex TicketTypeRegex = new(@"ticket type #(\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex QuantityRegex = new(@"Quantity (\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly ICustomerRepository _repository;

        public CustomerWorkflowService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public Task<User?> LoginAsync(string userName, string password)
        {
            return _repository.GetUserByCredentialsAsync(userName, password);
        }

        public async Task<PagedResult<EventListItem>> SearchEventsAsync(EventSearchRequest request)
        {
            var pageNumber = Math.Max(1, request.PageNumber);
            var pageSize = Math.Clamp(request.PageSize, 1, 50);
            var totalItems = await _repository.CountEventsAsync(request.Search, request.CategoryId, request.DateFilter, request.Location, request.TicketTypeId);
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
            pageNumber = Math.Min(pageNumber, totalPages);

            var events = await _repository.GetEventsAsync(
                request.Search,
                request.CategoryId,
                request.DateFilter,
                request.Location,
                request.TicketTypeId,
                (pageNumber - 1) * pageSize,
                pageSize);

            var items = events
                .Select(e => new EventListItem(e.Id, e.Name, e.EventCategory?.Name, e.Location, e.TimeStart, e.Status))
                .ToList();

            return new PagedResult<EventListItem>(
                items,
                pageNumber,
                totalPages,
                await _repository.GetCategoriesAsync(),
                await _repository.GetTicketTypesAsync(),
                await _repository.GetLocationsAsync());
        }

        public async Task<EventDetailsResult?> GetEventDetailsAsync(int eventId)
        {
            var eventItem = await _repository.GetEventWithCategoryAsync(eventId);
            if (eventItem == null)
            {
                return null;
            }

            var ticketTypes = await _repository.GetTicketTypesByEventAsync(eventId);
            return new EventDetailsResult(eventItem, ticketTypes);
        }

        public Task<List<EventCategory>> GetCategoriesAsync()
        {
            return _repository.GetCategoriesAsync();
        }

        public async Task<CategoryDetailsResult?> GetCategoryDetailsAsync(int categoryId)
        {
            var category = await _repository.GetCategoryAsync(categoryId);
            if (category == null)
            {
                return null;
            }

            var events = await _repository.GetEventsByCategoryAsync(categoryId);
            return new CategoryDetailsResult(category, events);
        }

        public async Task<CustomerBookingResult> BookTicketAsync(
            int userId,
            int ticketTypeId,
            int quantity,
            string? discountCode,
            int redeemPoints)
        {
            quantity = Math.Clamp(quantity, 1, 10);

            var ticketType = await _repository.GetTicketTypeWithEventAsync(ticketTypeId);
            if (ticketType == null || ticketType.Event == null)
            {
                return CustomerBookingResult.Fail("Ticket type not found.");
            }

            if (ticketType.Event.TimeStart <= DateTime.Now)
            {
                return CustomerBookingResult.Fail("This event is no longer available for booking.");
            }

            if (ticketType.Quantity < quantity)
            {
                return CustomerBookingResult.Fail("Not enough tickets are available.");
            }

            var user = await _repository.GetUserByIdAsync(userId);
            var membership = user?.MembershipId == null
                ? null
                : await _repository.GetMembershipByIdAsync(user.MembershipId.Value);

            var subtotal = ticketType.Price * quantity;
            var availablePoints = await CalculateAvailablePointsAsync(userId);
            var pointsToRedeem = Math.Clamp(redeemPoints, 0, availablePoints);
            var discountPercent = Math.Max(GetDiscountPercent(discountCode), GetTierDiscountPercent(membership));
            var percentDiscountAmount = decimal.Round(subtotal * discountPercent / 100m, 0);
            var pointDiscountAmount = Math.Min(pointsToRedeem * 1000m, subtotal - percentDiscountAmount);
            var finalAmount = Math.Max(0, subtotal - percentDiscountAmount - pointDiscountAmount);
            var earnedPoints = CalculatePoints(finalAmount);

            var transaction = new Transaction
            {
                UserId = userId,
                Status = TransactionStatusPending,
                Detail = $"Pending payment for ticket type #{ticketTypeId}. Quantity {quantity}. Amount {finalAmount:N0} VND. Discount {discountPercent}%. Redeemed {pointsToRedeem} point(s). Earned {earnedPoints} point(s)."
            };

            await using var dbTransaction = await _repository.BeginTransactionAsync();
            await _repository.AddTransactionAsync(transaction);
            await _repository.SaveChangesAsync();

            var booking = new Booking
            {
                UserId = userId,
                Status = BookingStatusPending,
                CreatedAt = DateTime.Now,
                TransactionId = transaction.Id
            };

            await _repository.AddBookingAsync(booking);
            await _repository.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return CustomerBookingResult.Ok(booking.Id);
        }

        public Task<List<Booking>> GetBookingHistoryAsync(int userId)
        {
            return _repository.GetBookingsByUserAsync(userId);
        }

        public async Task<BookingDetailsResult?> GetBookingDetailsAsync(int userId, int bookingId)
        {
            var booking = await _repository.GetBookingWithTransactionAsync(userId, bookingId);
            if (booking == null)
            {
                return null;
            }

            var tickets = await _repository.GetTicketsByBookingAsync(booking.Id);
            return new BookingDetailsResult(booking, tickets);
        }

        public async Task<CustomerPaymentResult> ConfirmTransactionAsync(int userId, int bookingId)
        {
            await using var dbTransaction = await _repository.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            var booking = await _repository.GetBookingWithTransactionAsync(userId, bookingId);
            if (booking == null || booking.Transaction == null)
            {
                return CustomerPaymentResult.Fail("Booking not found.");
            }

            var existingTickets = await _repository.CountTicketsByBookingAsync(booking.Id);
            if (booking.Status == BookingStatusPaid && booking.Transaction.Status == TransactionStatusConfirmed && existingTickets > 0)
            {
                await dbTransaction.CommitAsync();
                return CustomerPaymentResult.Ok(booking.Id, "Transaction is already confirmed.");
            }

            if (!TryReadPaymentDetail(booking.Transaction.Detail, out var ticketTypeId, out var quantity))
            {
                await dbTransaction.RollbackAsync();
                return CustomerPaymentResult.Fail("Cannot read booking payment detail.");
            }

            var ticketType = await _repository.GetTicketTypeWithEventAsync(ticketTypeId);
            if (ticketType == null || ticketType.Event == null)
            {
                await dbTransaction.RollbackAsync();
                return CustomerPaymentResult.Fail("Ticket type not found.");
            }

            if (ticketType.Event.TimeStart <= DateTime.Now)
            {
                await dbTransaction.RollbackAsync();
                return CustomerPaymentResult.Fail("This event is no longer available for payment confirmation.");
            }

            var missingTicketCount = Math.Max(0, quantity - existingTickets);
            if (missingTicketCount > 0)
            {
                var stockUpdated = await _repository.ReserveTicketQuantityAsync(ticketTypeId, missingTicketCount);
                if (stockUpdated == 0)
                {
                    await dbTransaction.RollbackAsync();
                    return CustomerPaymentResult.Fail("Not enough tickets are available.");
                }

                var tickets = Enumerable.Range(0, missingTicketCount)
                    .Select(_ => new Ticket
                    {
                        BookingId = booking.Id,
                        TicketTypeId = ticketTypeId,
                        IsCheckedIn = false
                    })
                    .ToList();

                await _repository.AddTicketsAsync(tickets);
                await _repository.SaveChangesAsync();

                foreach (var ticket in tickets)
                {
                    ticket.QR = BuildQrValue(ticket);
                }
            }

            booking.Status = BookingStatusPaid;
            booking.Transaction.Status = TransactionStatusConfirmed;
            booking.Transaction.Detail = MarkPaymentConfirmed(booking.Transaction.Detail);

            await _repository.SaveChangesAsync();
            await UpdateUserTierAsync(userId);
            await _repository.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return CustomerPaymentResult.Ok(booking.Id, "Payment confirmed. QR ticket has been issued.");
        }

        public Task<List<Ticket>> GetTicketsAsync(int userId)
        {
            return _repository.GetTicketsByUserAsync(userId);
        }

        public async Task<Ticket?> GetOwnedTicketAsync(int userId, int ticketId, bool ensureQr)
        {
            var ticket = await _repository.GetTicketWithDetailsAsync(ticketId);
            if (ticket?.Booking?.UserId != userId)
            {
                return null;
            }

            if (ensureQr && !IsSignedQr(ticket.QR, ticket.Id))
            {
                ticket.QR = BuildQrValue(ticket);
                await _repository.SaveChangesAsync();
            }

            return ticket;
        }

        public async Task<CustomerScanResult> ScanTicketAsync(string? qr)
        {
            qr = qr?.Trim();
            if (string.IsNullOrWhiteSpace(qr))
            {
                return new CustomerScanResult("Please enter or scan a QR code.", null);
            }

            if (!TryReadSignedQr(qr, out var ticketId))
            {
                return new CustomerScanResult("Invalid QR signature.", null);
            }

            var ticket = await _repository.GetTicketWithDetailsAsync(ticketId);
            if (ticket == null || ticket.QR != qr)
            {
                return new CustomerScanResult("Ticket not found.", null);
            }

            if (ticket.IsCheckedIn)
            {
                return new CustomerScanResult($"Ticket already checked in at {ticket.CheckInTime:g}.", ticket);
            }

            ticket.IsCheckedIn = true;
            ticket.CheckInTime = DateTime.Now;
            await _repository.SaveChangesAsync();

            return new CustomerScanResult("Ticket validated successfully.", ticket);
        }

        public async Task<RewardsSummary?> GetRewardsSummaryAsync(int userId)
        {
            await UpdateUserTierAsync(userId);
            await _repository.SaveChangesAsync();

            var user = await _repository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var membership = user.MembershipId == null
                ? null
                : await _repository.GetMembershipByIdAsync(user.MembershipId.Value);
            var earnedPoints = await CalculateEarnedPointsAsync(userId);
            var redeemedPoints = await CalculateRedeemedPointsAsync(userId);
            var availablePoints = Math.Max(0, earnedPoints - redeemedPoints);
            var tierDiscount = GetTierDiscountPercent(membership);

            return new RewardsSummary(user, membership, earnedPoints, redeemedPoints, availablePoints, tierDiscount);
        }

        public async Task<RewardRedemptionResult> ApplyRewardAsync(int userId, string? discountCode, int redeemPoints)
        {
            var availablePoints = await CalculateAvailablePointsAsync(userId);
            var pointsUsed = Math.Clamp(redeemPoints, 0, availablePoints);
            var discountPercent = GetDiscountPercent(discountCode);

            if (pointsUsed > 0 || discountPercent > 0)
            {
                await _repository.AddTransactionAsync(new Transaction
                {
                    UserId = userId,
                    Detail = $"Applied discount code '{discountCode?.Trim()}' ({discountPercent}%) and Redeemed {pointsUsed} point(s).",
                    Status = TransactionStatusConfirmed
                });

                await _repository.SaveChangesAsync();
                await UpdateUserTierAsync(userId);
                await _repository.SaveChangesAsync();
            }

            return new RewardRedemptionResult(discountPercent, pointsUsed, availablePoints);
        }

        public async Task<List<Membership>> GetMembershipsAsync()
        {
            var memberships = await _repository.GetMembershipsAsync();
            return memberships
                .OrderBy(GetTierThreshold)
                .ThenBy(m => m.Name)
                .ToList();
        }

        public async Task<int> CalculateAvailablePointsAsync(int userId)
        {
            var earnedPoints = await CalculateEarnedPointsAsync(userId);
            var redeemedPoints = await CalculateRedeemedPointsAsync(userId);
            return Math.Max(0, earnedPoints - redeemedPoints);
        }

        public async Task<int> CalculateEarnedPointsAsync(int userId)
        {
            var totalPaid = await _repository.GetPaidTicketTotalAsync(userId, new[] { BookingStatusPaid, 7 });
            return CalculatePoints(totalPaid);
        }

        public async Task<int> CalculateRedeemedPointsAsync(int userId)
        {
            var details = await _repository.GetConfirmedRedeemDetailsAsync(userId);
            return details.Sum(detail =>
            {
                var match = RedeemedPointsRegex.Match(detail);
                return match.Success ? int.Parse(match.Groups[1].Value) : 0;
            });
        }

        public async Task UpdateUserTierAsync(int userId)
        {
            var user = await _repository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return;
            }

            var availablePoints = await CalculateAvailablePointsAsync(userId);
            var tiers = await _repository.GetMembershipsAsync();
            var tier = tiers
                .OrderByDescending(GetTierThreshold)
                .FirstOrDefault(m => availablePoints >= GetTierThreshold(m));

            if (tier == null)
            {
                return;
            }

            var now = DateTime.Now;
            var currentTier = user.MembershipId == null
                ? null
                : tiers.FirstOrDefault(m => m.Id == user.MembershipId.Value);
            var isInitialAssignment = currentTier == null;
            var isUpgrade = currentTier != null
                && GetTierThreshold(tier) > GetTierThreshold(currentTier);
            var needsValidityInitialization = currentTier != null
                && GetTierThreshold(currentTier) > 0
                && (!user.MembershipStartedAt.HasValue || !user.MembershipExpiresAt.HasValue);

            if (!isInitialAssignment && !isUpgrade && !needsValidityInitialization)
            {
                return;
            }

            var targetTier = isUpgrade || isInitialAssignment ? tier : currentTier!;
            DateTime? expiresAt = GetTierThreshold(targetTier) > 0 ? now.AddMonths(12) : null;
            var changeType = isUpgrade ? "Upgrade" : "Initialization";
            var reason = isUpgrade
                ? $"Upgraded after reaching {availablePoints:N0} available point(s)."
                : "Initialized the 12-month membership validity period.";

            await _repository.UpdateUserMembershipAsync(userId, targetTier.Id, now, expiresAt);
            await _repository.AddMembershipHistoryAsync(new MembershipHistory
            {
                UserId = userId,
                PreviousMembershipId = currentTier?.Id,
                NewMembershipId = targetTier.Id,
                ChangedAt = now,
                ChangeType = changeType,
                Reason = reason
            });
        }

        public static string BookingStatusLabel(int status)
        {
            return status switch
            {
                BookingStatusPending => "Pending",
                1 => "Holding",
                BookingStatusPaid => "Paid",
                3 => "Cancelled",
                7 => "Completed",
                _ => $"Status {status}"
            };
        }

        public static string TransactionStatusLabel(int status)
        {
            return status switch
            {
                TransactionStatusPending => "Pending",
                TransactionStatusConfirmed => "Confirmed",
                _ => $"Status {status}"
            };
        }

        public static string EventStatusLabel(int status)
        {
            return status switch
            {
                0 => "Draft",
                1 => "Published",
                2 => "Cancelled",
                3 => "Completed",
                _ => $"Status {status}"
            };
        }

        public static int GetDiscountPercent(string? discountCode)
        {
            return discountCode?.Trim().ToUpperInvariant() switch
            {
                "PRN2026" => 20,
                "STUDENT" => 15,
                "SAVE10" => 10,
                _ => 0
            };
        }

        public static int GetTierDiscountPercent(Membership? membership)
        {
            var tier = $"{membership?.Tier} {membership?.Name}".ToUpperInvariant();
            if (tier.Contains("PLATINUM")) return 15;
            if (tier.Contains("GOLD")) return 10;
            if (tier.Contains("SILVER")) return 5;
            return 0;
        }

        public static int GetTierThreshold(Membership membership)
        {
            var tier = $"{membership.Tier} {membership.Name}".ToUpperInvariant();
            return tier switch
            {
                var x when x.Contains("PLATINUM") => 5000,
                var x when x.Contains("GOLD") => 2000,
                var x when x.Contains("SILVER") => 500,
                var x when x.Contains("BRONZE") => 0,
                _ => membership.Point
            };
        }

        internal static int CalculatePoints(decimal amount)
        {
            return (int)Math.Floor(amount / 100000m) * 10;
        }

        private static bool TryReadPaymentDetail(string? detail, out int ticketTypeId, out int quantity)
        {
            ticketTypeId = 0;
            quantity = 0;

            if (string.IsNullOrWhiteSpace(detail))
            {
                return false;
            }

            var ticketTypeMatch = TicketTypeRegex.Match(detail);
            var quantityMatch = QuantityRegex.Match(detail);

            if (!ticketTypeMatch.Success || !quantityMatch.Success)
            {
                return false;
            }

            return int.TryParse(ticketTypeMatch.Groups[1].Value, out ticketTypeId)
                && int.TryParse(quantityMatch.Groups[1].Value, out quantity)
                && ticketTypeId > 0
                && quantity > 0;
        }

        private static string MarkPaymentConfirmed(string? detail)
        {
            if (string.IsNullOrWhiteSpace(detail))
            {
                return "Payment confirmed.";
            }

            if (detail.Contains("Payment confirmed", StringComparison.OrdinalIgnoreCase))
            {
                return detail;
            }

            return detail.Replace("Pending payment", "Payment confirmed", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSignedQr(string? qr, int ticketId)
        {
            return TryReadSignedQr(qr, out var parsedTicketId) && parsedTicketId == ticketId;
        }

        private static bool TryReadSignedQr(string? qr, out int ticketId)
        {
            ticketId = 0;
            if (string.IsNullOrWhiteSpace(qr))
            {
                return false;
            }

            var parts = qr.Split(':');
            if (parts.Length != 5 || parts[0] != "TICKET")
            {
                return false;
            }

            if (!int.TryParse(parts[1], out ticketId)
                || !int.TryParse(parts[2], out _)
                || !int.TryParse(parts[3], out _))
            {
                return false;
            }

            var payload = $"{parts[1]}:{parts[2]}:{parts[3]}";
            var expected = SignQrPayload(payload);
            return string.Equals(parts[4], expected, StringComparison.OrdinalIgnoreCase);
        }

        private static string BuildQrValue(Ticket ticket)
        {
            var payload = $"{ticket.Id}:{ticket.BookingId}:{ticket.TicketTypeId}";
            return $"TICKET:{payload}:{SignQrPayload(payload)}";
        }

        private static string SignQrPayload(string payload)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(QrSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return Convert.ToHexString(hash)[..16];
        }
    }

    public record EventSearchRequest(
        string? Search,
        int? CategoryId,
        string? DateFilter,
        string? Location,
        int? TicketTypeId,
        int PageNumber = 1,
        int PageSize = 10);

    public record EventListItem(
        int Id,
        string Name,
        string? CategoryName,
        string? Location,
        DateTime TimeStart,
        int Status);

    public record PagedResult<T>(
        IList<T> Items,
        int PageNumber,
        int TotalPages,
        IList<EventCategory> Categories,
        IList<TicketType> TicketTypes,
        IList<string> Locations);

    public record EventDetailsResult(EventEntity EventItem, IList<TicketType> TicketTypes);

    public record CategoryDetailsResult(EventCategory Category, IList<EventEntity> Events);

    public record BookingDetailsResult(Booking Booking, IList<Ticket> Tickets)
    {
        public bool CanConfirmPayment =>
            Booking.Status != CustomerWorkflowService.BookingStatusPaid
            || Booking.Transaction?.Status != CustomerWorkflowService.TransactionStatusConfirmed
            || Tickets.Count == 0;
    }

    public record RewardsSummary(
        User User,
        Membership? Membership,
        int EarnedPoints,
        int RedeemedPoints,
        int AvailablePoints,
        int TierDiscount);

    public record RewardRedemptionResult(int DiscountPercent, int PointsUsed, int AvailablePoints)
    {
        public string Message => $"Discount code: {DiscountPercent}%. Redeemed points: {PointsUsed}/{AvailablePoints}.";
    }

    public record CustomerBookingResult(bool Success, string? Error, int? BookingId)
    {
        public static CustomerBookingResult Ok(int bookingId) => new(true, null, bookingId);

        public static CustomerBookingResult Fail(string error) => new(false, error, null);
    }

    public record CustomerScanResult(string Message, Ticket? Ticket);

    public record CustomerPaymentResult(bool Success, string? Error, int? BookingId, string? Message)
    {
        public static CustomerPaymentResult Ok(int bookingId, string message) => new(true, null, bookingId, message);

        public static CustomerPaymentResult Fail(string error) => new(false, error, null, null);
    }
}

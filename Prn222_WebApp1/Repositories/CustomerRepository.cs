using System.Data;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MVC.Data2;
using EventEntity = Domain.Event;

namespace Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DemoMVC2Context _context;

        public CustomerRepository(DemoMVC2Context context)
        {
            _context = context;
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return _context.Database.BeginTransactionAsync(isolationLevel);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public Task<User?> GetUserByCredentialsAsync(string userName, string password)
        {
            return _context.User
                .FirstOrDefaultAsync(x => x.UserName == userName && x.Password == password);
        }

        public Task<User?> GetUserByIdAsync(int userId)
        {
            return _context.User.FirstOrDefaultAsync(x => x.Id == userId);
        }

        public Task<Membership?> GetMembershipByIdAsync(int membershipId)
        {
            return _context.Membership.FirstOrDefaultAsync(x => x.Id == membershipId);
        }

        public Task<List<Membership>> GetMembershipsAsync()
        {
            return _context.Membership.ToListAsync();
        }

        public async Task UpdateUserMembershipAsync(
            int userId,
            int membershipId,
            DateTime? startedAt,
            DateTime? expiresAt)
        {
            var user = await GetUserByIdAsync(userId);
            if (user != null)
            {
                user.MembershipId = membershipId;
                user.MembershipStartedAt = startedAt;
                user.MembershipExpiresAt = expiresAt;
            }
        }

        public Task<List<int>> GetExpiredMembershipUserIdsAsync(DateTime asOf)
        {
            return _context.User
                .Where(u => u.MembershipId != null
                    && u.MembershipExpiresAt != null
                    && u.MembershipExpiresAt <= asOf)
                .Select(u => u.Id)
                .ToListAsync();
        }

        public Task AddMembershipHistoryAsync(MembershipHistory history)
        {
            _context.MembershipHistory.Add(history);
            return Task.CompletedTask;
        }

        public Task<int> CountEventsAsync(string? search, int? categoryId, string? dateFilter, string? location, int? ticketTypeId)
        {
            return BuildEventQuery(search, categoryId, dateFilter, location, ticketTypeId).CountAsync();
        }

        public Task<List<EventEntity>> GetEventsAsync(string? search, int? categoryId, string? dateFilter, string? location, int? ticketTypeId, int skip, int take)
        {
            return BuildEventQuery(search, categoryId, dateFilter, location, ticketTypeId)
                .Include(e => e.EventCategory)
                .OrderBy(e => e.TimeStart)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public Task<EventEntity?> GetEventWithCategoryAsync(int eventId)
        {
            return _context.Event
                .Include(e => e.EventCategory)
                .FirstOrDefaultAsync(e => e.Id == eventId);
        }

        public Task<List<TicketType>> GetTicketTypesByEventAsync(int eventId)
        {
            return _context.TicketType
                .Where(t => t.EventId == eventId)
                .OrderBy(t => t.Price)
                .ToListAsync();
        }

        public Task<List<EventCategory>> GetCategoriesAsync()
        {
            return _context.EventCategory
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public Task<EventCategory?> GetCategoryAsync(int categoryId)
        {
            return _context.EventCategory.FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        public Task<List<EventEntity>> GetEventsByCategoryAsync(int categoryId)
        {
            return _context.Event
                .Where(e => e.EventCategoryId == categoryId)
                .OrderBy(e => e.TimeStart)
                .ToListAsync();
        }

        public Task<List<TicketType>> GetTicketTypesAsync()
        {
            return _context.TicketType
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public Task<List<string>> GetLocationsAsync()
        {
            return _context.Event
                .Where(e => e.Location != null && e.Location != string.Empty)
                .Select(e => e.Location!)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
        }

        public Task<TicketType?> GetTicketTypeWithEventAsync(int ticketTypeId)
        {
            return _context.TicketType
                .Include(t => t.Event)
                .FirstOrDefaultAsync(t => t.Id == ticketTypeId);
        }

        public Task AddTransactionAsync(Transaction transaction)
        {
            _context.Transaction.Add(transaction);
            return Task.CompletedTask;
        }

        public Task AddBookingAsync(Booking booking)
        {
            _context.Booking.Add(booking);
            return Task.CompletedTask;
        }

        public Task AddTicketsAsync(IEnumerable<Ticket> tickets)
        {
            _context.Ticket.AddRange(tickets);
            return Task.CompletedTask;
        }

        public Task<int> ReserveTicketQuantityAsync(int ticketTypeId, int quantity)
        {
            return _context.TicketType
                .Where(t => t.Id == ticketTypeId && t.Quantity >= quantity)
                .ExecuteUpdateAsync(setters => setters.SetProperty(t => t.Quantity, t => t.Quantity - quantity));
        }

        public Task<List<Booking>> GetBookingsByUserAsync(int userId)
        {
            return _context.Booking
                .Include(b => b.Transaction)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public Task<Booking?> GetBookingWithTransactionAsync(int userId, int bookingId)
        {
            return _context.Booking
                .Include(b => b.Transaction)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);
        }

        public Task<int> CountTicketsByBookingAsync(int bookingId)
        {
            return _context.Ticket.CountAsync(t => t.BookingId == bookingId);
        }

        public Task<List<Ticket>> GetTicketsByBookingAsync(int bookingId)
        {
            return _context.Ticket
                .Include(t => t.TicketType)
                    .ThenInclude(t => t!.Event)
                .Where(t => t.BookingId == bookingId)
                .OrderBy(t => t.Id)
                .ToListAsync();
        }

        public Task<List<Ticket>> GetTicketsByUserAsync(int userId)
        {
            return _context.Ticket
                .Include(t => t.Booking)
                .Include(t => t.TicketType)
                    .ThenInclude(t => t!.Event)
                .Where(t => t.Booking != null && t.Booking.UserId == userId)
                .OrderByDescending(t => t.Id)
                .ToListAsync();
        }

        public Task<Ticket?> GetTicketWithDetailsAsync(int ticketId)
        {
            return _context.Ticket
                .Include(t => t.Booking)
                .Include(t => t.TicketType)
                    .ThenInclude(t => t!.Event)
                .FirstOrDefaultAsync(t => t.Id == ticketId);
        }

        public async Task<decimal> GetPaidTicketTotalAsync(
            int userId,
            IReadOnlyCollection<int> paidStatuses,
            DateTime? fromInclusive = null,
            DateTime? toExclusive = null)
        {
            var query = _context.Ticket
                .Where(t => t.Booking != null
                    && t.Booking.UserId == userId
                    && paidStatuses.Contains(t.Booking.Status));

            if (fromInclusive.HasValue)
            {
                query = query.Where(t => t.Booking!.CreatedAt >= fromInclusive.Value);
            }

            if (toExclusive.HasValue)
            {
                query = query.Where(t => t.Booking!.CreatedAt < toExclusive.Value);
            }

            return await query.Select(t => (decimal?)t.TicketType!.Price)
                .SumAsync() ?? 0m;
        }

        public Task<List<string>> GetConfirmedRedeemDetailsAsync(int userId)
        {
            return _context.Transaction
                .Where(t => t.UserId == userId
                    && t.Status == 1
                    && t.Detail != null
                    && t.Detail.Contains("Redeemed"))
                .Select(t => t.Detail!)
                .ToListAsync();
        }

        private IQueryable<EventEntity> BuildEventQuery(string? search, int? categoryId, string? dateFilter, string? location, int? ticketTypeId)
        {
            var query = _context.Event.AsQueryable();
            var now = DateTime.Now;
            var today = now.Date;

            query = dateFilter switch
            {
                "today" => query.Where(e => e.TimeStart >= today && e.TimeStart < today.AddDays(1)),
                "past" => query.Where(e => e.TimeStart < now),
                "all" => query,
                _ => query.Where(e => e.TimeStart >= now)
            };

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim();
                query = query.Where(e =>
                    e.Name.Contains(keyword) ||
                    (e.Detail != null && e.Detail.Contains(keyword)) ||
                    (e.Location != null && e.Location.Contains(keyword)));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(e => e.EventCategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                var place = location.Trim();
                query = query.Where(e => e.Location != null && e.Location.Contains(place));
            }

            if (ticketTypeId.HasValue)
            {
                query = query.Where(e => _context.TicketType.Any(t => t.EventId == e.Id && t.Id == ticketTypeId.Value));
            }

            return query;
        }
    }
}

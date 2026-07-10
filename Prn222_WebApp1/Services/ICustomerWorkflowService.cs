using Domain;

namespace Services
{
    public interface ICustomerWorkflowService
    {
        Task<User?> LoginAsync(string userName, string password);
        Task<PagedResult<EventListItem>> SearchEventsAsync(EventSearchRequest request);
        Task<EventDetailsResult?> GetEventDetailsAsync(int eventId);
        Task<List<EventCategory>> GetCategoriesAsync();
        Task<CategoryDetailsResult?> GetCategoryDetailsAsync(int categoryId);
        Task<CustomerBookingResult> BookTicketAsync(int userId, int ticketTypeId, int quantity, string? discountCode, int redeemPoints);

        Task<List<Booking>> GetBookingHistoryAsync(int userId);
        Task<BookingDetailsResult?> GetBookingDetailsAsync(int userId, int bookingId);
        Task<CustomerPaymentResult> ConfirmTransactionAsync(int userId, int bookingId);

        Task<List<Ticket>> GetTicketsAsync(int userId);
        Task<Ticket?> GetOwnedTicketAsync(int userId, int ticketId, bool ensureQr);
        Task<CustomerScanResult> ScanTicketAsync(string? qr);

        Task<RewardsSummary?> GetRewardsSummaryAsync(int userId);
        Task<RewardRedemptionResult> ApplyRewardAsync(int userId, string? discountCode, int redeemPoints);
        Task<List<Membership>> GetMembershipsAsync();
        Task<int> CalculateAvailablePointsAsync(int userId);
        Task<int> CalculateEarnedPointsAsync(int userId);
        Task<int> CalculateRedeemedPointsAsync(int userId);
        Task UpdateUserTierAsync(int userId);
    }
}

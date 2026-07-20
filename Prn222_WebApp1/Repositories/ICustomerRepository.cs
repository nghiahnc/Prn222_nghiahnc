using System.Data;
using Domain;
using Microsoft.EntityFrameworkCore.Storage;
using EventEntity = Domain.Event;

namespace Repositories
{
    public interface ICustomerRepository
    {
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        Task SaveChangesAsync();

        Task<User?> GetUserByCredentialsAsync(string userName, string password);
        Task<User?> GetUserByIdAsync(int userId);
        Task<Membership?> GetMembershipByIdAsync(int membershipId);
        Task<List<Membership>> GetMembershipsAsync();
        Task UpdateUserMembershipAsync(int userId, int membershipId, DateTime? startedAt, DateTime? expiresAt);
        Task<List<int>> GetExpiredMembershipUserIdsAsync(DateTime asOf);
        Task AddMembershipHistoryAsync(MembershipHistory history);

        Task<int> CountEventsAsync(string? search, int? categoryId, string? dateFilter, string? location, int? ticketTypeId);
        Task<List<EventEntity>> GetEventsAsync(string? search, int? categoryId, string? dateFilter, string? location, int? ticketTypeId, int skip, int take);
        Task<EventEntity?> GetEventWithCategoryAsync(int eventId);
        Task<List<TicketType>> GetTicketTypesByEventAsync(int eventId);
        Task<List<EventCategory>> GetCategoriesAsync();
        Task<EventCategory?> GetCategoryAsync(int categoryId);
        Task<List<EventEntity>> GetEventsByCategoryAsync(int categoryId);
        Task<List<TicketType>> GetTicketTypesAsync();
        Task<List<string>> GetLocationsAsync();
        Task<TicketType?> GetTicketTypeWithEventAsync(int ticketTypeId);

        Task AddTransactionAsync(Transaction transaction);
        Task AddBookingAsync(Booking booking);
        Task AddTicketsAsync(IEnumerable<Ticket> tickets);
        Task<int> ReserveTicketQuantityAsync(int ticketTypeId, int quantity);

        Task<List<Booking>> GetBookingsByUserAsync(int userId);
        Task<Booking?> GetBookingWithTransactionAsync(int userId, int bookingId);
        Task<int> CountTicketsByBookingAsync(int bookingId);
        Task<List<Ticket>> GetTicketsByBookingAsync(int bookingId);
        Task<List<Ticket>> GetTicketsByUserAsync(int userId);
        Task<Ticket?> GetTicketWithDetailsAsync(int ticketId);

        Task<decimal> GetPaidTicketTotalAsync(
            int userId,
            IReadOnlyCollection<int> paidStatuses,
            DateTime? fromInclusive = null,
            DateTime? toExclusive = null);
        Task<List<string>> GetConfirmedRedeemDetailsAsync(int userId);
    }
}

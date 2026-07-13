using Domain;

namespace Services
{
    public interface IOrganizerReportService
    {
        Task<OrganizerActivity> GetOrganizerActivityAsync(int organizerId);

        Task<List<TicketSaleStatistic>> GetTicketSalesStatisticsAsync(int organizerId);

        Task<List<EventRevenue>> GetEventRevenueAsync(int organizerId);

        Task<OrganizerReport> GetOrganizerReportAsync(int organizerId);
    }
}

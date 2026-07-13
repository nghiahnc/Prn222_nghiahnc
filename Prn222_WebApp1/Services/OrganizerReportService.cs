using Repositories;
using Domain;

namespace Services
{
    public class OrganizerReportService : IOrganizerReportService
    {
        private readonly IOrganizerReportRepository _repository;

        public OrganizerReportService(IOrganizerReportRepository repository)
        {
            _repository = repository;
        }

        public Task<OrganizerActivity> GetOrganizerActivityAsync(int organizerId)
        {
            return _repository.GetOrganizerActivityAsync(organizerId);
        }

        public Task<List<TicketSaleStatistic>> GetTicketSalesStatisticsAsync(int organizerId)
        {
            return _repository.GetTicketSalesStatisticsAsync(organizerId);
        }

        public Task<List<EventRevenue>> GetEventRevenueAsync(int organizerId)
        {
            return _repository.GetEventRevenueAsync(organizerId);
        }

        public Task<OrganizerReport> GetOrganizerReportAsync(int organizerId)
        {
            return _repository.GetOrganizerReportAsync(organizerId);
        }
    }
}

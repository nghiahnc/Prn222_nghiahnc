using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class TicketIssueService : ITicketIssueService
    {
        private readonly ITicketIssueRepository _repository;
        private readonly IQRCodeService _qrCodeService;

        public TicketIssueService(
            ITicketIssueRepository repository,
            IQRCodeService qrCodeService)
        {
            _repository = repository;
            _qrCodeService = qrCodeService;
        }

        public async Task<bool> GenerateTicketsAsync(int bookingId, int organizerId)
        {
            var booking = await _repository.GetBookingForTicketIssueAsync(
                bookingId,
                organizerId);

            if (booking == null)
            {
                return false;
            }

            if (booking.Transaction == null || booking.Transaction.Status != 1)
            {
                return false;
            }

            var index = 1;

            foreach (var ticket in booking.Tickets)
            {
                if (!string.IsNullOrEmpty(ticket.QR))
                {
                    continue;
                }

                ticket.QR = _qrCodeService.GenerateTicketCode(
                    booking.Id,
                    ticket.TicketTypeId,
                    index);

                ticket.IsCheckedIn = false;
                index++;
            }

            await _repository.SaveChangesAsync();

            return true;
        }
    }
}

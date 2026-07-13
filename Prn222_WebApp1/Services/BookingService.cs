using Domain;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IEmailService _emailService;

        public BookingService(
            IBookingRepository bookingRepository,
            IEmailService emailService)
        {
            _bookingRepository = bookingRepository;
            _emailService = emailService;
        }

        public async Task CreateBookingAsync(Booking booking)
        {
            await _bookingRepository.CreateAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            var bookingWithUser =
                await _bookingRepository.GetBookingWithUserAsync(booking.Id);

            var user = bookingWithUser?.User;

            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                return;
            }

            await _emailService.SendBookingConfirmationAsync(
                user.Email,
                user.UserName,
                booking.Id);

            await _bookingRepository.MarkConfirmationSentAsync(booking.Id);
        }
    }
}

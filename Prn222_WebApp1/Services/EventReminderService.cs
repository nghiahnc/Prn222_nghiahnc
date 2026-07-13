using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class EventReminderService : IEventReminderService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IEmailService _emailService;

        public EventReminderService(
            IBookingRepository bookingRepository,
            IEmailService emailService)
        {
            _bookingRepository = bookingRepository;
            _emailService = emailService;
        }

        public async Task SendEventRemindersAsync()
        {
            var reminderTime = DateTime.Now.AddMinutes(5);

            var bookings =
                await _bookingRepository.GetBookingsNeedReminderAsync(reminderTime);

            foreach (var booking in bookings)
            {
                var user = booking.User;

                if (user == null || string.IsNullOrEmpty(user.Email))
                {
                    continue;
                }

                var eventName = booking.Tickets
                    .First()
                    .TicketType!
                    .Event!
                    .Name;

                await _emailService.SendEventReminderAsync(
                    user.Email,
                    user.UserName,
                    eventName);

                await _bookingRepository.MarkReminderSentAsync(booking.Id);
            }
        }
    }
}

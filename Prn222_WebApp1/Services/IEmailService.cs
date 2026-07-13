namespace Services
{
    public interface IEmailService
    {
        Task SendBookingConfirmationAsync(
            string toEmail,
            string customerName,
            int bookingId);

        Task SendEventReminderAsync(
            string toEmail,
            string customerName,
            string eventName);

        Task SendMassNotificationAsync(
            string toEmail,
            string subject,
            string message);
    }
}

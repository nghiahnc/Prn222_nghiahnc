using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendBookingConfirmationAsync(
            string toEmail,
            string customerName,
            int bookingId)
        {
            await SendEmailAsync(
                toEmail,
                "Booking Confirmation",
                $@"
            <h2>Xin chào {customerName}</h2>
            <p>Bạn đã đặt vé thành công.</p>
            <p>Mã booking: <b>#{bookingId}</b></p>
            <p>Cảm ơn bạn đã sử dụng dịch vụ.</p>");
        }

        public async Task SendEventReminderAsync(
            string toEmail,
            string customerName,
            string eventName)
        {
            await SendEmailAsync(
                toEmail,
                "Event Reminder",
                $@"
            <h2>Xin chào {customerName}</h2>
            <p>Sự kiện <b>{eventName}</b> sẽ diễn ra trong vòng 24 giờ tới.</p>
            <p>Đừng quên tham gia nhé.</p>");
        }

        public async Task SendMassNotificationAsync(
            string toEmail,
            string subject,
            string message)
        {
            await SendEmailAsync(toEmail, subject, message);
        }

        private async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body)
        {
            var email = new MimeMessage();

            email.From.Add(
                new MailboxAddress(
                    _settings.SenderName,
                    _settings.SenderEmail));

            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            email.Body = new TextPart("html")
            {
                Text = body
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _settings.SenderEmail,
                _settings.Password);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }
}

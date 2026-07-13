using Repositories;


namespace Services
{

    public class NotificationService : INotificationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public NotificationService(
            IUserRepository userRepository,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task SendMassNotificationAsync(string subject, string message)
        {
            var users = await _userRepository.GetUsersHaveEmailAsync();

            foreach (var user in users)
            {
                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    continue;
                }

                await _emailService.SendMassNotificationAsync(
                    user.Email,
                    subject,
                    message);
            }
        }
    }
}

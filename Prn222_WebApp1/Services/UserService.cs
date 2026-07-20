using Domain;
using Repositories;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public List<User> GetAllUsers() => _repo.GetAll();

        public User? GetUserById(int id) => _repo.GetById(id);

        public void CreateUser(User user) => _repo.Create(user);

        public void UpdateUser(User user) => _repo.Update(user);

        public void DeleteUser(int id) => _repo.Delete(id);


        /// <summary>Ban a user with reason validation (B.E Custom Validation).</summary>
        public ServiceResult BanUser(int userId, string reason)
        {
            // reason required
            if (string.IsNullOrWhiteSpace(reason))
                return ServiceResult.Fail("A reason must be provided when banning an account.");

            if (reason.Trim().Length < 10)
                return ServiceResult.Fail("Ban reason must be at least 10 characters long.");

            var user = _repo.GetById(userId);
            if (user is null)
                return ServiceResult.Fail($"User with ID {userId} was not found.");

            // cannot ban an Admin
            if (user.Role == 1)
                return ServiceResult.Fail("Administrator accounts cannot be banned.");

            // already banned
            if (user.Status == 2)
                return ServiceResult.Fail($"Account '{user.UserName}' is already banned.");

            user.Status = 2; // Banned
            _repo.Update(user);
            return ServiceResult.Ok();
        }

        /// <summary>Reactivate a banned/pending user.</summary>
        public ServiceResult ActivateUser(int userId)
        {
            var user = _repo.GetById(userId);
            if (user is null)
                return ServiceResult.Fail($"User with ID {userId} was not found.");

            if (user.Role == 1)
                return ServiceResult.Fail("Administrator accounts are always active.");

            if (user.Status == 1)
                return ServiceResult.Fail($"Account '{user.UserName}' is already active.");

            user.Status = 1; // Active
            _repo.Update(user);
            return ServiceResult.Ok();
        }

        /// <summary>Approve a pending organizer registration.</summary>
        public ServiceResult ApproveOrganizer(int userId)
        {
            var user = _repo.GetById(userId);
            if (user is null)
                return ServiceResult.Fail($"Organizer with ID {userId} was not found.");

            if (user.Role != 2)
                return ServiceResult.Fail("Only organizer accounts can be approved.");

            if (user.Status != 0)
                return ServiceResult.Fail($"Organizer '{user.UserName}' is not in Pending status.");

            user.Status = 1; // Active/Approved
            _repo.Update(user);
            return ServiceResult.Ok();
        }

        /// <summary>Reject a pending organizer registration.</summary>
        public ServiceResult RejectOrganizer(int userId)
        {
            var user = _repo.GetById(userId);
            if (user is null)
                return ServiceResult.Fail($"Organizer with ID {userId} was not found.");

            if (user.Role != 2)
                return ServiceResult.Fail("Only organizer accounts can be rejected.");

            if (user.Status == 2)
                return ServiceResult.Fail($"Organizer '{user.UserName}' is already rejected/banned.");

            user.Status = 2; // Rejected / Banned
            _repo.Update(user);
            return ServiceResult.Ok();
        }

        /// <summary>Get count of pending organizer registrations (used by Worker).</summary>
        public int GetPendingOrganizerCount()
        {
            return _repo.GetAll().Count(u => u.Role == 2 && u.Status == 0);
        }
    }
}
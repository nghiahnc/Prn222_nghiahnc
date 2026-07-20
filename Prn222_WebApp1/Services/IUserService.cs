using Domain;
using System.Collections.Generic;

namespace Services
{
    public interface IUserService
    {
        List<User> GetAllUsers();
        User? GetUserById(int id);
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int id);

        ServiceResult BanUser(int userId, string reason);
        ServiceResult ActivateUser(int userId);
        ServiceResult ApproveOrganizer(int userId);
        ServiceResult RejectOrganizer(int userId);
        int GetPendingOrganizerCount();
    }
}
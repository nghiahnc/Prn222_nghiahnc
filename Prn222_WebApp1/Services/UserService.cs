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

        public List<User> GetAllUsers()
        {
            return _repo.GetAll();
        }

        public User? GetUserById(int id)
        {
            return _repo.GetById(id);
        }

        public void CreateUser(User user)
        {
            _repo.Create(user);
        }

        public void UpdateUser(User user)
        {
            _repo.Update(user);
        }

        public void DeleteUser(int id)
        {
            _repo.Delete(id);
        }
    }
}
using Domain;
using Microsoft.EntityFrameworkCore;
using MVC.Data2;


namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DemoMVC2Context _context;

        public UserRepository(DemoMVC2Context context)
        {
            _context = context;
        }

        public List<User> GetAll()
        {
            return _context.User.ToList();
        }

        public User? GetById(int id)
        {
            return _context.User.FirstOrDefault(u => u.Id == id);
        }

        public void Create(User user)
        {
            _context.User.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            _context.User.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.User.Find(id);

            if (user != null)
            {
                _context.User.Remove(user);
                _context.SaveChanges();
            }
        }

        public async Task<List<User>> GetUsersHaveEmailAsync()
        {
            return await _context.User
                .Where(u => !string.IsNullOrEmpty(u.Email))
                .ToListAsync();
        }
    }
}
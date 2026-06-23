using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
namespace Repositories
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User? GetById(int id);
        void Create(User user);
        void Update(User user);
        void Delete(int id);
    }

}

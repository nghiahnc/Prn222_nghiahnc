using Domain;
using System.Collections.Generic;

namespace Repositories
{
    public interface ITransactionRepository
    {
        List<Transaction> GetAll();
        Transaction? GetById(int id);
        void Create(Transaction transaction);
        void Update(Transaction transaction);
        void Delete(int id);
    }
}

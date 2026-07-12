using Domain;
using Microsoft.EntityFrameworkCore;
using MVC.Data2;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly DemoMVC2Context _context;

        public TransactionRepository(DemoMVC2Context context)
        {
            _context = context;
        }

        public List<Transaction> GetAll()
        {
            return _context.Transaction.Include(t => t.User).ToList();
        }

        public Transaction? GetById(int id)
        {
            return _context.Transaction.Include(t => t.User).FirstOrDefault(t => t.Id == id);
        }

        public void Create(Transaction transaction)
        {
            _context.Transaction.Add(transaction);
            _context.SaveChanges();
        }

        public void Update(Transaction transaction)
        {
            _context.Transaction.Update(transaction);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var transaction = _context.Transaction.FirstOrDefault(t => t.Id == id);
            if (transaction != null)
            {
                _context.Transaction.Remove(transaction);
                _context.SaveChanges();
            }
        }
    }
}

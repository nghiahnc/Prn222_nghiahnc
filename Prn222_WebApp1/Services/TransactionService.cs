using Domain;
using Repositories;
using System.Collections.Generic;

namespace Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repo;

        public TransactionService(ITransactionRepository repo)
        {
            _repo = repo;
        }

        public List<Transaction> GetAllTransactions()
        {
            return _repo.GetAll();
        }

        public Transaction? GetTransactionById(int id)
        {
            return _repo.GetById(id);
        }

        public void CreateTransaction(Transaction transaction)
        {
            _repo.Create(transaction);
        }

        public void UpdateTransaction(Transaction transaction)
        {
            _repo.Update(transaction);
        }

        public void DeleteTransaction(int id)
        {
            _repo.Delete(id);
        }
    }
}

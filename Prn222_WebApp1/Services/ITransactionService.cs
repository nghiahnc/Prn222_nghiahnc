using Domain;
using System.Collections.Generic;

namespace Services
{
    public interface ITransactionService
    {
        List<Transaction> GetAllTransactions();
        Transaction? GetTransactionById(int id);
        void CreateTransaction(Transaction transaction);
        void UpdateTransaction(Transaction transaction);
        void DeleteTransaction(int id);
    }
}

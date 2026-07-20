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

        public List<Transaction> GetAllTransactions() => _repo.GetAll();

        public Transaction? GetTransactionById(int id) => _repo.GetById(id);

        public void CreateTransaction(Transaction transaction) => _repo.Create(transaction);

        public void UpdateTransaction(Transaction transaction) => _repo.Update(transaction);

        public void DeleteTransaction(int id) => _repo.Delete(id);

        /// <summary>
        /// Resolve a transaction dispute 
        /// Valid newStatus: 4 = Refunded, 5 = Dispute Rejected.
        /// </summary>
        public ServiceResult ResolveDispute(int transactionId, int newStatus)
        {
            //only allow status 4 or 5
            if (newStatus != 4 && newStatus != 5)
                return ServiceResult.Fail(
                    "Invalid resolution status. Allowed values: 4 (Refund Approved) or 5 (Dispute Rejected).");

            var transaction = _repo.GetById(transactionId);
            if (transaction is null)
                return ServiceResult.Fail($"Transaction #{transactionId} was not found.");

            //must be in dispute state
            if (transaction.Status != 3)
                return ServiceResult.Fail(
                    $"Transaction #{transactionId} is not in 'In Dispute' status (current status: {transaction.Status}). Only disputed transactions can be resolved.");

            transaction.Status = newStatus;
            _repo.Update(transaction);
            return ServiceResult.Ok();
        }

        /// <summary>Get count of open disputes (Status = 3). Used by Worker + NavMenu badge.</summary>
        public int GetOpenDisputeCount()
        {
            return _repo.GetAll().Count(t => t.Status == 3);
        }
    }
}

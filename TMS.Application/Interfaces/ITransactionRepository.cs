using TMS.Domain.Entities;
using TMS.Domain.Enums;


namespace TMS.Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddTransaction(Transaction transaction);
        Task<Transaction?> GetTransactionById(Guid transactionId);
        Task<List<Transaction>> GetTransactionsByTellerId(Guid tellerId);
        Task<List<Transaction>> GetAllTransactions(TransactionType? type, DateTime? fromUtc, DateTime? toUtc);
    }
}

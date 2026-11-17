using TMS.Domain.Entities;
using TMS.Domain.Enums;


namespace TMS.Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddTransactionAsync(Transaction transaction);
        Task<List<Transaction>> GetAllTransactionsAsync(TransactionType? type, DateTime? fromUtc, DateTime? toUtc);
        Task<Transaction?> GetTransactionByIdAsync(Guid transactionId);
        Task<List<Transaction>> GetTransactionsByTellerIdAsync(Guid tellerId);
    }
}

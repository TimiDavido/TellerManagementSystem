using TMS.Application.DTOs;
using TMS.Domain.Entities;
using TMS.Domain.Enums;

namespace TMS.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> CreateTransactionAsync(CreateTransactionDto dto, Guid TellerId);
        Task<List<TransactionDto>> GetAllTransactionsAsync(TransactionType? transactionType, DateTime? fromUtc, DateTime? toUtc);
        Task<TransactionDto?> GetTransactionByIdAsync(Guid transactionId, Guid userId, string userRole);
        Task<List<TransactionDto>> GetTransactionByTellerIdAsync(Guid tellerId);
    }
}

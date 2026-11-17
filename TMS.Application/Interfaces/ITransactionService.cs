using System;
using System.Collections.Generic;
using System.Text;

using TMS.Application.DTOs;
using TMS.Domain.Entities;
using TMS.Domain.Enums;


namespace TMS.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> CreateTransaction(CreateTransactionDto dto, Guid TellerId);
        Task<List<Transaction>> GetTransactionByTellerId(Guid TellerId);
        Task<List<Transaction>> GetAllTransactions(TransactionType? type, DateTime? fromUtc, DateTime? toUtc);
        Task<Transaction> GetTransactionById(Guid transactionId);
    }
}

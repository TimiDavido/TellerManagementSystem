using System;
using System.Collections.Generic;
using System.Text;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;
using TMS.Domain.Entities;
using TMS.Domain.Enums;

namespace TMS.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountValidationService _accountValidationService;

        public TransactionService(ITransactionRepository transactionRepository, IAccountValidationService accountValidationService)
        {
            _transactionRepository = transactionRepository;
            _accountValidationService = accountValidationService;
        }

        public async Task<Transaction> CreateTransactionAsync(CreateTransactionDto dto, Guid TellerId)
        {
            if (dto.Amount <= 0)
            {
                throw new ArgumentException("Transaction must be greater than zero.");
            }

            if (!Enum.IsDefined(typeof(TransactionType), dto.TransactionType))
            {
                throw new ArgumentException("Invalid transaction type.");
            }

            var account = await _accountValidationService.ValidateAccount(dto.AccountNumber);

            if (account.Result == null)
            {
                throw new InvalidOperationException("Account validation failed.");
            }

            decimal balanceBefore = account.Result.Balance;
            decimal balanceAfter;

            switch (dto.TransactionType)
            {
                case TransactionType.Deposit:
                    balanceAfter = balanceBefore + dto.Amount;
                    break;

                case TransactionType.Withdraw:
                    if (balanceBefore < dto.Amount)
                    {
                        throw new InvalidOperationException("Insufficient funds for withdrawal.");
                    }
                    balanceAfter = balanceBefore - dto.Amount;
                    break;

                default:
                    throw new ArgumentException("Invalid transaction type.");
            }

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountNumber = dto.AccountNumber,
                AccountName = account.Result.AccTitle,
                Amount = dto.Amount,
                TransactionType = dto.TransactionType,
                TellerId = TellerId,
                CreatedAt = DateTime.UtcNow,
                Description = dto.Description
            };

            await _transactionRepository.AddTransactionAsync(transaction);

            return transaction;
        }

        public async Task<List<TransactionDto>> GetAllTransactionsAsync(TransactionType? transactionType, DateTime? fromUtc, DateTime? toUtc)
        {
            var transactions = await _transactionRepository.GetAllTransactionsAsync(transactionType, fromUtc, toUtc);
            return transactions.Select(MapToDto).ToList();
        }

        public async Task<TransactionDto?> GetTransactionByIdAsync(Guid transactionId, Guid userId, string userRole)
        {
            var transaction  = await _transactionRepository.GetTransactionByIdAsync(transactionId);

            if (transaction is null)
                return null;

            if (userRole == "Teller" && transaction.TellerId != userId)
                return null;

            return MapToDto(transaction);
        }

        public async Task<List<TransactionDto>> GetTransactionByTellerIdAsync(Guid tellerId)
        {
            var transactions = await _transactionRepository.GetTransactionsByTellerIdAsync(tellerId);
            return transactions.Select(MapToDto).ToList();
        }

        private static TransactionDto MapToDto(Transaction t) => new()
        {
            Id = t.Id,
            TellerId = t.TellerId,
            AccountNumber = t.AccountNumber,
            AccountName = t.AccountName,
            Amount = t.Amount,
            TransactionType = t.TransactionType.ToString(),
            CreatedAt = t.CreatedAt,
            Description = t.Description,
        };
    }
}

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

        public async Task<Transaction> CreateTransaction(CreateTransactionDto dto, Guid TellerId)
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

                case TransactionType.Withdrawal:
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

            await _transactionRepository.AddTransaction(transaction);

            return transaction;
        }
    }
}

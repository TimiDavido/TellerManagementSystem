using System;
using System.Collections.Generic;
using System.Text;

using TMS.Application.DTOs;
using TMS.Domain.Entities;

namespace TMS.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> CreateTransaction(CreateTransactionDto dto, Guid TellerId);
    }
}

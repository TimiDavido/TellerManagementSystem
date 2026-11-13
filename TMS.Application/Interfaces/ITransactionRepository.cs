using System;
using System.Collections.Generic;
using System.Text;
using TMS.Domain.Entities;


namespace TMS.Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddTransaction(Transaction transaction);
    }
}

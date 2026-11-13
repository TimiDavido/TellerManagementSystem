using System;
using System.Collections.Generic;
using System.Text;
using TMS.Application.DTOs;

namespace TMS.Application.Interfaces
{
    public interface IAccountValidationService
    {
        Task<AccountBalanceResponseDto> ValidateAccount(string accountNumber);
    }
}

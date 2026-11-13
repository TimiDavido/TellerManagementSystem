using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Application.DTOs
{
    public class AccountBalanceResponseDto
    {
            public AccountDetailsDto? Result { get; set; }
            public string? ResponseCode { get; set; }
            public string? ResponseMessage { get; set; }
    }

    public class AccountDetailsDto
    {
            public string? AccountNo { get; set; }
            public string? AccTitle { get; set; }
            public string? CurrencyCode { get; set; }
            public decimal Balance { get; set; }
    }
}

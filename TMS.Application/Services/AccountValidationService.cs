using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;

namespace TMS.Application.Services
{
    public class AccountValidationService : IAccountValidationService
    {
        private readonly HttpClient _httpClient;

        public AccountValidationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AccountBalanceResponseDto> ValidateAccount(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length != 10 || !accountNumber.All(char.IsAsciiDigit))
            {
                throw new ArgumentException("Account must contain exactly 10 digits.");
            }

            var response = await _httpClient.GetAsync($"api/Operations/GetAccountBalance/{accountNumber}");

            response.EnsureSuccessStatusCode(); 

            var result = await response.Content.ReadFromJsonAsync<AccountBalanceResponseDto>();

            if (result == null)
            {
                throw new Exception("The bank returned an empty response.");
            }

            if (result.ResponseCode != "00" || result.Result == null)
            {
                throw new Exception($"Account validation failed.");
            }

            return result;
        }
    }
}

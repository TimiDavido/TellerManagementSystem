using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;

namespace TMS.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _httpClient;

        public EmailService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SendSingleEmailResponse> SendEmail(SendSingleEmailRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Notify/SendSingleMail", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<SendSingleEmailResponse>();
                return result!;
            }
            else
            {
                return new SendSingleEmailResponse
                {
                    ResponseCode = (int)response.StatusCode,
                    ResponseMessage = response.ReasonPhrase
                };
            }
        }
    }
}
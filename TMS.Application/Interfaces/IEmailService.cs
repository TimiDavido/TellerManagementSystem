using System;
using System.Collections.Generic;
using System.Text;
using TMS.Application.DTOs;

namespace TMS.Application.Interfaces
{
    public interface IEmailService
    {
        Task<SendSingleEmailResponse> SendEmail(SendSingleEmailRequest request);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TMS.Application.DTOs;

namespace TMS.Application.Interfaces
{
    public interface ILoginService
    {
        Task<LoginResponseDto> Login(string email, string password);
    }
}

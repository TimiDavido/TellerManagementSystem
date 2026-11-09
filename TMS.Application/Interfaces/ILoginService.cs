using System;
using System.Collections.Generic;
using System.Text;
using TMS.Application.DTOs;

namespace TMS.Application.Interfaces
{
    public interface ILoginService
    {
<<<<<<< HEAD
        Task<LoginResponseDto> Login(string email, string password);
=======
        Task<string> Login(string email, string password);   
>>>>>>> origin/feature/requests-transactions
    }
}

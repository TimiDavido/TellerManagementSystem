using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Application.Interfaces
{
    public interface ILoginService
    {
        Task<string> Login(string email, string password);
    }
}

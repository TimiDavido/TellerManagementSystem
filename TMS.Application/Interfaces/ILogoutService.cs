using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Application.Interfaces
{
    public interface ILogoutService
    {
        Task Logout(string refreshToken);
    }
}

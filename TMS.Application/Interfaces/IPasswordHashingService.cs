using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Application.Interfaces
{
    public interface IPasswordHashingService
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string password);
    }
}


using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Application.Interfaces;

namespace TMS.Application.Services
{
    public class PasswordHashingService : IPasswordHashingService
    {
        private readonly PasswordHasher<object> _passwordHasher = new();

        public  string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null!, password);
        }
        public bool VerifyPassword(string hashedPassword, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(null!, hashedPassword, password);
            return result != PasswordVerificationResult.Failed;
        }

    }
}

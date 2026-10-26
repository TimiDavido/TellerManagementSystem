using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TMS.Application.Interfaces;

namespace TMS.Application.Services
{
    public class TemporaryPasswordService : ITemporaryPasswordService
    {
        public string GeneratePassword()
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(12);
            return Convert.ToBase64String(randomBytes);
        }
    }
}

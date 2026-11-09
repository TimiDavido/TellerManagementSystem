using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        string GenerateRefreshToken();
        string HashToken(string token);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TMS.Domain.Entities;

namespace TMS.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenHash(string tokenHash);
        Task AddToken(RefreshToken refreshToken);
        Task UpdateToken(RefreshToken refreshToken);
    }
}

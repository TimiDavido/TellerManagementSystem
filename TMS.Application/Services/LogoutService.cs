using System;
using System.Collections.Generic;
using System.Text;
using TMS.Application.Interfaces;

namespace TMS.Application.Services
{
    public class LogoutService : ILogoutService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRefreshTokenService _refreshTokenService;

        public LogoutService(IRefreshTokenRepository refreshTokenRepository, IRefreshTokenService refreshTokenService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _refreshTokenService = refreshTokenService;
        }

        public async Task Logout(string refreshToken)
        {
            var tokenHash = _refreshTokenService.HashToken(refreshToken);
            var storedToken = await _refreshTokenRepository.GetByTokenHash(tokenHash);

            if(storedToken == null)
            {
                return;
            }

            if(storedToken.RevokedAt != null)
            {
                return;
            }

            storedToken.RevokedAt = DateTime.UtcNow;

            await _refreshTokenRepository.UpdateToken(storedToken);
        }
    }
}

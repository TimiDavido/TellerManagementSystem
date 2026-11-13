using System;
using System.Collections.Generic;
using System.Text;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;
using TMS.Domain.Entities;

namespace TMS.Application.Services
{
    public class RefreshTokenApplicationService : IRefreshTokenApplicationService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public RefreshTokenApplicationService(
            IRefreshTokenRepository refreshTokenRepository,IRefreshTokenService refreshTokenService,IUserRepository userRepository,IJwtTokenService jwtTokenService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _refreshTokenService = refreshTokenService;
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginResponseDto> RefreshToken(string refreshToken)
        {
            var tokenHash = _refreshTokenService.HashToken(refreshToken);

            var storedToken = await _refreshTokenRepository.GetByTokenHash(tokenHash);

            if (storedToken == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }
              
            if (storedToken.RevokedAt != null)
            {
                throw new UnauthorizedAccessException("Refresh token has been revoked.");
            }
                
            if (storedToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token has expired.");
            }
                
            var user = await _userRepository.GetUserById(storedToken.UserId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }
                
            if (user.AccountStatus != "ACTIVE")
            {
                throw new UnauthorizedAccessException("Account is not active.");
            }

            var role = await _userRepository.GetRoleNameById(user.RoleId);

            if (role == null)
            {
                throw new InvalidOperationException("User role not found.");
            }

            var newAccessToken = _jwtTokenService.GenerateToken( user.Id, user.Email, role);

            var newRefreshToken = _refreshTokenService.GenerateRefreshToken();

            var newRefreshTokenHash = _refreshTokenService.HashToken(newRefreshToken);

            var newRefreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = newRefreshTokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            storedToken.RevokedAt = DateTime.UtcNow;

            await _refreshTokenRepository.UpdateToken(storedToken);
            await _refreshTokenRepository.AddToken(newRefreshTokenEntity);

            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

    }
}

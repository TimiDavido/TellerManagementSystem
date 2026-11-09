using System;
using System.Collections.Generic;
using System.Text;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;
using TMS.Domain.Entities;

namespace TMS.Application.Services
{
    public class LoginService : ILoginService
    {
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public LoginService(IPasswordHashingService passwordHashingService, IUserRepository userRepository, IJwtTokenService jwtTokenService, IRefreshTokenService refreshTokenService, IRefreshTokenRepository refreshTokenRepository)
        {
            _passwordHashingService = passwordHashingService;
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<LoginResponseDto> Login(string email, string password)
        {
            var user = await _userRepository.GetUserByEmail(email);

            if (user == null)
            {
                throw new Exception("Invalid email or password.");
            }

            bool passwordIsValid = _passwordHashingService.VerifyPassword(user.PasswordHash, password);

            if (!passwordIsValid)
            {
                throw new Exception("Invalid email or password.");
            }

            if (user.AccountStatus != "ACTIVE")
            {
                throw new Exception("Account is not active, contact the HOP");
            }

            var role = await _userRepository.GetRoleNameById(user.RoleId);

            if (role == null)
            {
                throw new Exception("User role not found.");
            }

            var accessToken = _jwtTokenService.GenerateToken(user.Id, user.Email, role);

            var refreshToken = _refreshTokenService.GenerateRefreshToken();

            var refreshTokenHash = _refreshTokenService.HashToken(refreshToken);

            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _refreshTokenRepository.AddToken(refreshTokenEntity);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}

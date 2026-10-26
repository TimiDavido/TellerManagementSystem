using System;
using System.Collections.Generic;
using System.Text;
using TMS.Application.Interfaces;

namespace TMS.Application.Services
{
    public class LoginService : ILoginService
    {
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginService(IPasswordHashingService passwordHashingService, IUserRepository userRepository, IJwtTokenService jwtTokenService)
        {
            _passwordHashingService = passwordHashingService;
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<string> Login(string email, string password)
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
                throw new Exception("Account is not active.");
            }

            var role = await _userRepository.GetRoleNameById(user.RoleId);

            if (role == null)
            {
                throw new Exception("User role not found.");
            }

            return _jwtTokenService.GenerateToken(user.Id, user.Email, role);
        }
    }
}

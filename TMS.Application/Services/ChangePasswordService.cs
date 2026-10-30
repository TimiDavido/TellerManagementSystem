using System;
using System.Collections.Generic;
using System.Text;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;

namespace TMS.Application.Services
{
    public class ChangePasswordService : IChangePasswordService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashingService _passwordHashingService;

        public ChangePasswordService(IUserRepository userRepository, IPasswordHashingService passwordHashingService)
        {
            _userRepository = userRepository;
            _passwordHashingService = passwordHashingService;
        }

        public async Task<bool> ChangePassword(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetUserById(userId);

            if (user == null)
            {
                return false;
            }

            bool currentPasswordIsValid = _passwordHashingService.VerifyPassword(user.PasswordHash, currentPassword);

            if (!currentPasswordIsValid)
            {
                return false;
            }

            user.PasswordHash = _passwordHashingService.HashPassword(newPassword);

            await _userRepository.UpdateUser(user);

            return true;
        }
    }
}

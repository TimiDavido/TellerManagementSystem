using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;

namespace TMS.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ITemporaryPasswordService _temporaryPasswordService;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;

        public UserService(IPasswordHashingService passwordHashingService, ITemporaryPasswordService temporaryPasswordService, IEmailService emailService, IUserRepository userRepository)
        {
            _passwordHashingService = passwordHashingService;
            _temporaryPasswordService = temporaryPasswordService;
            _emailService = emailService;
            _userRepository = userRepository;
        }

        public async Task<UserDto> CreateUser(CreateUserDto createUserDto, Guid createdBy)
        {
            string temporaryPassword = _temporaryPasswordService.GeneratePassword();
            string hashedPassword = _passwordHashingService.HashPassword(temporaryPassword);

            var createdUser = await _userRepository.CreateUser(createUserDto, createdBy, hashedPassword);

            var emailRequest = new SendSingleEmailRequest
            {
                Receiver = createdUser.Email,
                Subject = "Your Teller account has been created",
                Body = $"""
                       Hello {createdUser.FirstName} {createdUser.LastName},

                       Your Teller account has been successfully created.

                       Your temporary password is: {temporaryPassword}

                       Please log in and change your password as soon as possible.

                       Regards,
                       The TMS Team
                       """
            };
            var emailResponse = await _emailService.SendEmail(emailRequest);
            return createdUser;

        }
    }
}

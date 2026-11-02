using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;
using TMS.Domain.Entities;
using TMS.Infrastructure.Data;

namespace TMS.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ITemporaryPasswordService _temporaryPasswordService;
        public UserRepository(AppDbContext context, IPasswordHashingService passwordHashingService, ITemporaryPasswordService temporaryPasswordService)
        {
            _context = context;
            _passwordHashingService = passwordHashingService;
            _temporaryPasswordService = temporaryPasswordService;
        }
        public async Task<UserDto> CreateUser(CreateUserDto createUserDto, Guid createdBy)
        {
            bool emailExists = await _context.Users.AnyAsync(user => user.Email == createUserDto.Email);

            if (emailExists)
            {
                throw new Exception("Email already exists.");
            }

           var tellerRole = await _context.Roles.FirstOrDefaultAsync(role => role.RoleName == "TELLER");

            if (tellerRole == null)
            {
                throw new Exception("Teller role not found in the database.");
            }

           string defaultPassword = _temporaryPasswordService.GeneratePassword();
           string hashedPassword = _passwordHashingService.HashPassword(defaultPassword);

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email,
                RoleId = tellerRole.Id,
                PasswordHash = hashedPassword,
                AccountStatus = "INACTIVE",
                CreatedBy = createdBy
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                RoleId = user.RoleId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                AccountStatus = user.AccountStatus,
                CreatedBy = user.CreatedBy
            };
        }

        public async Task<User?> GetUserById(Guid userId)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
        }
        public async Task<List<UserDto>> GetAllUsers()
        {
            return await _context.Users
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    RoleId = user.RoleId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    AccountStatus = user.AccountStatus,
                    CreatedBy = user.CreatedBy
                })
                .ToListAsync();
        }

        public async Task<string?> GetRoleNameById(int roleId)
        {
            return await _context.Roles
                .Where(role => role.Id == roleId)
                .Select(role => role.RoleName)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TMS.Application.DTOs;
using TMS.Domain.Entities;

namespace TMS.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDto> CreateUser(CreateUserDto createUserDto);
        Task<User?> GetUserByEmail(string email);
        Task<List<UserDto>> GetAllUsers();
        Task<string?> GetRoleNameById(int roleId);
    }
}

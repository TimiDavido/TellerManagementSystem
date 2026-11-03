using System;
using System.Collections.Generic;
using System.Text;
using TMS.Application.DTOs;

namespace TMS.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> CreateUser(CreateUserDto createUserDto, Guid createdBy);
    }
}

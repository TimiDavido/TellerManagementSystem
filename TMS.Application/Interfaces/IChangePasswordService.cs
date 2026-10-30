using System;
using System.Collections.Generic;
using System.Text;
using TMS.Application.DTOs;

namespace TMS.Application.Interfaces
{
    public interface IChangePasswordService
    {
        Task<bool> ChangePassword(Guid userId, string currentPassword, string newPassword);
    }
}

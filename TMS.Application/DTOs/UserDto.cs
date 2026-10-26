using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AccountStatus { get; set; } = string.Empty;
        public Guid? CreatedBy { get; set; }
    }
}

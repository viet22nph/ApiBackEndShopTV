using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.UserManagement
{
    public class AddRoleToUserRequest
    {
        [Required(ErrorMessage ="User id not null or empty")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Role name not null or empty")]
        public string RoleName { get; set; }

    }
}

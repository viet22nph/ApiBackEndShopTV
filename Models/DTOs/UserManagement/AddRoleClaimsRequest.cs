using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.UserManagement
{
    public class AddRoleClaimsRequest
    {
        public string RoleName { get; set; }
        public ICollection<ClaimsRequest> RoleClaims { get; set; }

    }
    public class ClaimsRequest
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}

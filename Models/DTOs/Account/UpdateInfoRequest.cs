using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Account
{
    public class UpdateInfoRequest
    {
        public string Id {  get; set; }
        public string? FullName { get; set; }
        [Required]
        [MinLength(6)]
        public string UserName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string DisplayName { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
    }

}

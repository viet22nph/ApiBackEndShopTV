using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Banner.Request
{
    public class BannerRequestDto
    {
        [Required]
        [MinLength(6)]
        [MaxLength(120)]
        public string Url { get; set; }
        public bool IsEnable { get; set; } = false;
        [Required]
        public Guid GroupId { get; set; }

    }
}

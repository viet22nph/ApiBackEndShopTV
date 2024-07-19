using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.GroupBanner
{
    public class GroupBannerDto
    {
        public Guid Id { get; set; }
        public string GroupName { get; set; }

        public bool IsEnable { get; set; } = false;
        public DateTime DateCreate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.GroupBanner
{
    public class GroupBannerDetailDto
    {
        public Guid Id { get; set; }

        public string GroupName { get; set; }
        public bool IsEnable { get; set; }
        public DateTime DateCreate { get; set; }
        public ICollection<Banner> Banners { get; set; }
        public class Banner {
            public Guid Id { get; set; }
            public string Url { get; set; }
            public bool IsEnable { get; set; }
        }

    }
}

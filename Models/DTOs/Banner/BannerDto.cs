using Models.DTOs.GroupBanner;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Banner
{
    public class BannerDto 
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public bool IsEnable { get; set; }
        public DateTime DateCreate { get; set; }
        public GroupBannerDto GroupBanner { get; set; }
    }
}

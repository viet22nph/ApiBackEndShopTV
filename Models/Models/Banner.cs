using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Banner: BaseEntity
    {
        public string Url {  get; set; }
        public bool IsEnable { get; set; } = false;
        public GroupBanner? Group { get; set; }
        public Guid GroupId { get; set; }

    }
    public class GroupBanner:  BaseEntity
    {
        public string GroupName { get; set; }
        public bool IsEnable { get; set; } = false;

        public ICollection<Banner>? Banners { get; set; }
    }

}

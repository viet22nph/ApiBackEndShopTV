using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Tag: BaseEntity
    {
        public string TagTitle { get; set; }
        public virtual ICollection<TagBlog>? TagBlogs { get; set; }
    }

    public class TagBlog
    {
        public Guid TagId { get; set; }
        public Guid BlogId { get; set; }

        public Tag Tag { get; set; }
        public Blog Blog { get; set; }
    }
}

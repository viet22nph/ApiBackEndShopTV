using Models.DbEntities;

namespace Models.Models
{
    public class BlogGroup : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; } 
        public virtual ICollection<Blog>? Blogs { get; set;}
    }
}

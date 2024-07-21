using Models.DbEntities;


namespace Models.Models
{
    public class Blog: BaseEntity
    {
        public string BlogImage { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string AuthorId { get; set; }
        public Guid BlogGroupId { get; set; }
        public BlogGroup BlogGroup { get; set; }
        public ApplicationUser Author { get; set; }
        public ICollection<TagBlog>? TagBlogs { get; set;}

    }
}

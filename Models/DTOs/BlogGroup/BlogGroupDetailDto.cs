using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.BlogGroup
{
    public class BlogGroupDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<BlogDto> Blogs { get; set; }
        public string Slug { get; set; }
        public class BlogDto
        {
            public Guid Id { get; set; }
            public string BlogImage { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            public string AuthorId { get; set; }
            public List<string> Tags { get; set; }
            public string Slug { get; set; }
        }

    }


}

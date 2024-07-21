using Models.DTOs.BlogGroup;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Blog
{
    public class BlogDto
    {
      
            public Guid Id { get; set; }
            public string BlogImage { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            public string AuthorId { get; set; }
            public string AuthorName { get; set; }
            public Guid BlogGroupId { get; set; }
            public string BlogGroupName { get; set; }
            public List<string> Tags { get; set; } = new List<string>();
      
    }
}

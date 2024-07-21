using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Blog.Request
{
    public class BlogRequestDto
    {
        [Required]
        public string BlogImage { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public string AuthorId { get; set; }
        [Required]
        public Guid BlogGroupId { get; set; }
        public ICollection<string>? TagsBlog { get; set; }
    }
}

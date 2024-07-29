using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Category
{
    public class CategoryDto
    {
        public string Id { get; set;  }
        public string Name { get; set; }
        public Guid? CategoryParent { get; set; }
        public string? Description { get; set; }
        public string Slug { get; set; }
    }
}

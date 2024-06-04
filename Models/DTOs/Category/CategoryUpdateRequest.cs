using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Category
{
    public class CategoryUpdateRequest
    {
        public string? Name { get; set; }
        public Guid? CategoryParent { get; set; }
        public string? Description { get; set; }
    }
}

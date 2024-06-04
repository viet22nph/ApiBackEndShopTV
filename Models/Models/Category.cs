using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DAL.Models
{
    public class Category: BaseEntity
    {
        public string Name { get; set; }
        public Guid? CategoryParent { get; set; }
        public string? Description { get; set; }
        public string NomalizedName { get; set; }
        public Category CategoryParentNavigation { get; set; }
        public ICollection<Category>? CategoryChildren { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}

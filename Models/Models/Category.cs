using Models.DbEntities;
using Newtonsoft.Json;
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
        [JsonIgnore]

        public virtual  Category CategoryParentNavigation { get; set; }
        [JsonIgnore]

        public virtual ICollection<Category>? CategoryChildren { get; set; }
        [JsonIgnore]

        public virtual ICollection<Product>? Products { get; set; }
    }
}

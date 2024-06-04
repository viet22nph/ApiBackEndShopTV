using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DAL.Models
{
    public class Supplier: BaseEntity
    {
        public string? ContactPhone { get; set; }
        public string ContactPerson { get; set; }
        public string SupplierName { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }

        public ICollection<Product> Products { get; set;}
    }
}

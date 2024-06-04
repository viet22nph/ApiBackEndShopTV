using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Product
{
    public class ProductSpecificationDto
    {
        public Guid Id { get; set; }
        public string SpecValue { get; set; }
        public string SpecType { get; set; }
    }
}

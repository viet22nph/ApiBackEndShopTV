using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Product
{
    public class ProductUpdateRequest
    {

        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ProductQuantity { get; set; }
        public string? ProductBrand { get; set; }
        public decimal? Price { get; set; }
        public Guid? SupplierId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? DiscountId { get; set; }
        public List<ProductSpecificationUpdateRequest>? ProductSpecifications { get; set; }
        public List<ProductItemUpdateRequest>? ProductItems { get; set; }
    }
}

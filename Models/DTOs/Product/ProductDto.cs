using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int ProductQuantity { get; set; }
        public string? ProductBrand { get; set; }
        public decimal Price { get; set; }
        public Guid? SupplierId { get; set; }
        public Guid CategoryId { get; set; }
        public ICollection<ProductSpecificationDto>? ProductSpecifications { get; set; }
        public ICollection<ProductItemDto>? ProductItems { get; set; }
    }
}

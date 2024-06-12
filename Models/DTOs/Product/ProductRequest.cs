using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Product
{
    public class ProductRequest
    {
        [Required(ErrorMessage ="Product name not null")]
        [MinLength(3, ErrorMessage = "Product name must be longer than 3 characters")]
        //[RegularExpression(@"^[a-zA-Z0-9\s\-_/,.]*$", ErrorMessage = "Product name cannot contain special characters")]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "Product quantity not null")]
        [Range(1, int.MaxValue, ErrorMessage = "Product quantity must be at least 1")]
        public int ProductQuantity { get; set; }
        //[RegularExpression(@"^[a-zA-Z0-9\s\-_/.,]*$", ErrorMessage = "Product name cannot contain special characters")]
        public string? ProductBrand { get; set; }

        [Required(ErrorMessage = "Product name not null")]
        [Range(1000, int.MaxValue, ErrorMessage = "Product price must be at least 1000")]
        public decimal Price { get; set; }
        public Guid? SupplierId { get; set; }
        [Required(ErrorMessage = "Product must belong to 1 category")]
        public Guid CategoryId { get; set; }
        public Guid? DiscountId { get; set; }
        public ICollection<ProductSpecificationRequest>? ProductSpecifications { get; set; }
        public ICollection<ProductItemRequest> ProductItems { get; set; }
    }
}

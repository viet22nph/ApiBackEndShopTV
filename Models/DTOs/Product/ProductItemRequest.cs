using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Product
{
    public class ProductItemRequest
    {
        [Required(ErrorMessage = "Product Items quantity not null")]
        [Range(1, int.MaxValue, ErrorMessage = "Product quantity must be at least 1")]
        public int Quantity { get; set; }
        [Required(ErrorMessage ="Color not null")]
        public Guid? ColorId { get; set; }
        public ICollection<ProductImageRequest> ProductImages { get; set; }
    }
    public class ProductImageRequest
    {
        public string Url { get; set; }
    }
}

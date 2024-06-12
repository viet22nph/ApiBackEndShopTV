using Models.DbEntities;
using Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DAL.Models
{
    public class Product: BaseEntity
    {
        public string Name { get; set; }
        public string? NormalizedName { get; set; }
        public string? Description { get; set; }
        public int ProductQuantity { get; set; }
        public string? ProductBrand { get; set; }
        public decimal Price { get; set; }
        public bool IsDraft { get; set; } = true;
        public bool IsPublished { get; set; } = false;

        public Guid? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        public Guid? DiscountId { get; set; }
        public Discount Discount { get; set; }
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<ProductSpecification>? ProductSpecifications { get; set; }
        public ICollection<ProductItem>? ProductItems { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
        
    }
    public class ProductSpecification:BaseEntity
    {
        public Guid ProductId { get; set; }
        public string SpecValue { get; set; }
        public string SpecType { get; set; }

        public Product? Product { get; set; }

    }
    public class ProductItem:BaseEntity
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid? ColorId { get; set; }

        public Product? Product { get; set; }
        public ICollection<ProductImage>? ProductImages { get; set; }
        public Color? Color { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
    public class Color: BaseEntity
    {
        public string ColorName { get; set; }
        [StringLength(256)]
        public string ColorCode { get; set; }
        public ICollection<ProductItem>? ProductItems { get; set; }
    }
    public class ProductImage: BaseEntity
    {
        public Guid ProductItemId { get; set; }
        public string Url { get; set; }
        public ProductItem? ProductItem { get; set; }
    }

    

}

using Models.DbEntities;
using Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
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
        public string Slug { get; set; }
        public Guid? SupplierId { get; set; }
        [JsonIgnore]
        public Supplier? Supplier { get; set; }
        public Guid? DiscountId { get; set; }
        [JsonIgnore]
        public Discount? Discount { get; set; }
        public Guid CategoryId { get; set; }
        [JsonIgnore]
        public Category? Category { get; set; }
        [JsonIgnore]
        public  virtual ICollection<ProductSpecification>? ProductSpecifications { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProductItem>? ProductItems { get; set; }
        [JsonIgnore]
        public virtual ICollection<Review>? Reviews { get; set; }
        
    }
    public class ProductSpecification:BaseEntity
    {
        public Guid ProductId { get; set; }
        public string SpecValue { get; set; }
        public string SpecType { get; set; }
        [JsonIgnore]
        public  virtual Product? Product { get; set; }

    }
    public class ProductItem:BaseEntity
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid? ColorId { get; set; }
        [JsonIgnore]
        public virtual Product? Product { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProductImage>? ProductImages { get; set; }
        [JsonIgnore]
        public virtual Color? Color { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderItem>? OrderItems { get; set; }
    }
    public class Color: BaseEntity
    {
        public string ColorName { get; set; }
        [StringLength(256)]
        public string ColorCode { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProductItem>? ProductItems { get; set; }
    }
    public class ProductImage: BaseEntity
    {
        public Guid ProductItemId { get; set; }
        public string Url { get; set; }
        [JsonIgnore]
        public virtual ProductItem? ProductItem { get; set; }
    }

    

}

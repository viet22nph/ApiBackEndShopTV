﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string CategoryName { get; set; }
        public string SupplierName { get; set; }

        public string Slug { get; set; }
        public ICollection<ProductSpecificationDto>? ProductSpecifications { get; set; }
        public ICollection<ProductItemDto>? ProductItems { get; set; }

       
    }


    public class ProductIdDto
    {
        public Guid ProductId { get; set; }
    }

    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string? Description { get; set; }
        public int ProductQuantity { get; set; }
        public string? ProductBrand { get; set; }
        public decimal Price { get; set; }

        public CategoryProduct Category { get; set; }
        public SupplierProduct? Supplier { get; set; }
        public string Image { get; set; }
        public ICollection<ProductSpecificationDto>? ProductSpecifications { get; set; }
        public ICollection<ProductItemResponse>? ProductItems { get; set; }
        public Rating? Rating { get; set; }
        public ProductDiscount? ProductDiscount { get; set; }
        public bool IsPublished { get; set; }

        public class CategoryProduct
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
        public class SupplierProduct
        {
            public Guid Id { get; set; }
            public string SupplierName { get; set; }
        }
    }

    public class ProductItemResponse
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public ColorResponse? Color { get; set; }

        public ICollection<ProductImageDto>? ProductImages { get; set; }
    }

    public class ColorResponse
    {
        public Guid Id { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
    }



    public partial class Rating
    {
        public float? Rate { get; set; }
        public int? Count { get; set; }
    }
    public partial class ProductDiscount
    {
        public Guid? Id { get; set; }
        public string? Type { get; set; }
        public decimal? Value { get; set; }
    }
}

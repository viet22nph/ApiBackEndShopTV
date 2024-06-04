using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ResponseModels
{
    public class OrderResponse
    {
        public Guid OrderId { get; set; }
        public string OrderType { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string RecipientName { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Notes { get; set; }
        public ICollection<OrderItemResponse>? OrderItems { get; set; }
        public TransactionResponse? Transaction { get; set; }
    }

    public class OrderItemResponse
    {
        public Guid ProductItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public ProductItemResponse? Product { get; set; }
    }

    public class ProductItemResponse
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public ColorResponse? Color { get; set; }
        public ICollection<ProductImageResponse>? ProductImages { get; set; }
    }
    public class ColorResponse
    {
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
    }

    public class ProductImageResponse
    {
        public string Url { get; set; }
    }

    public class TransactionResponse
    {
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // COD - MOMO - VN pay
        public string? Description { get; set; }
        public string Status { get; set; }
    }


}

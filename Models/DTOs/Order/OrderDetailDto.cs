using Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Order
{
    public class OrderDetailDto
    {
        public Guid OrderId { get; set; }
        public string? UserId { get; set; }
        public string OrderType { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string RecipientName { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public decimal? TotalDiscount { get; set; }
        public decimal? TotalShip { get; set; }
        public string Status { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? DateUpdate { get; set; }
        public string? Notes { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
        public class OrderItem
        {
            public Guid OrderId { get; set; }
            public Guid ProductItemId { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal AmountDiscount { get; set; }
            public ProductItem? Product { get; set; }
            public class ProductItem
            {
                public Guid ProductId { get; set; }
                public string ProductName { get; set; }
                public Color? ColorItem { get; set; }
                public string? Image {  get; set; }
                public class Color
                {
                    public Guid Id { get; set; }
                    public string ColorName { get; set; }
                    public string ColorCode { get; set; }
                }
            }
        }

        public ICollection<Transaction>? Transactions { get; set; }
        public class Transaction
        {
            public Guid Id { get; set; }
            public DateTime DateCreate {  get; set; }
            public DateTime? DateUpdate { get; set; }
            public string? UserId { get; set; }
            public decimal Amount { get; set; }
            public string Type { get; set; } // COD - MOMO - VN pay
            public string? Description { get; set; }
            public string Status { get; set; }
        }


    }
}

using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Order
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string OrderType { get; set; }// Offline or online
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? RecipientName { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; }
        public string? UserId { get; set; }
        public ICollection<OrderItemDto>? OrderItems { get; set; }
        //public TransactionDto Transaction { get; set; }
    }
    public class OrderItemDto
    {
        public Guid ProductItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
    public class TransactionDto
    {

        public decimal Amount { get; set; }
        public string Type { get; set; } // COD - MOMO - VN pay
        public string? Description { get; set; }
        public string? UserId { get; set; }
        public string Status { get; set; }
    }
}

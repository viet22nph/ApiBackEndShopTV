using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Order
{
    public class OrderRequest
    {
        [Required(ErrorMessage = "Order type")]
        public string OrderType { get; set; }// Offline or online
        [Required(ErrorMessage ="Address")]
        public string Address { get; set; }
        [Required(ErrorMessage ="Phone")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Recipient Name")]
        public string RecipientName { get; set; }
        [Required(ErrorMessage = "SubTotal")]
        public decimal SubTotal { get; set; }
        [Required(ErrorMessage = "Grand toal")]
        public decimal GrandTotal { get; set; }
        public string? Notes { get; set; }
        [Required(ErrorMessage ="Items")]
        public ICollection<OrderItemDto> Items { get; set; }

        public string? UserId { get; set; }
        public TransactionDto Transaction { get; set; }
    }
    public class OrderItemRequest
    {
        public Guid ProductItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
    public class TransactionRequest
    {
        public decimal Amount { get; set; }
        [Required(ErrorMessage ="Type payment")]
        public string Type { get; set; } // COD - MOMO - VN pay
        public string? Description { get; set; }
        public string? UserId { get; set; }
        public string Status { get; set; }
    }
}

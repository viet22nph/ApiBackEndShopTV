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
        [Required(ErrorMessage = "Order type Required")]
        public string OrderType { get; set; }// Offline or online
        [Required(ErrorMessage = "Address required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Phone required")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Recipient Name required")]
        public string RecipientName { get; set; }
        [Required(ErrorMessage = "SubTotal required")]
        public decimal SubTotal { get; set; }
        [Required(ErrorMessage = "Total required")]
        public decimal Total { get; set; }
        public decimal? TotalDiscount { get; set; }
        public string? Notes { get; set; }
        [Required(ErrorMessage = "Items required")]
        public ICollection<OrderItemRequest> Items { get; set; }
        public class OrderItemRequest
        {
            public Guid ProductItemId { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal AmountDiscount { get; set; }
        }

        public string? UserId { get; set; }

        public ICollection<TransactionRequest> Transactions { get; set; }
        public class TransactionRequest
        {
            public decimal Amount { get; set; }
            [Required(ErrorMessage = "Type payment")]
            public string Type { get; set; } // COD - MOMO - VN pay
            public string? Description { get; set; }
            public string? UserId { get; set; }
            public string Status { get; set; }
        }
    }
}

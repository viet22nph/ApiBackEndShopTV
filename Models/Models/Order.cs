using Models.DbEntities;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DAL.Models
{
    public class Order: BaseEntity
    {
        public string OrderType { get; set; }// Offline or online
        public string Address { get; set; }
        public string Phone { get; set; }
        public string RecipientName { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public string Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? Notes { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }
        public Transaction? Transaction{ get; set; }
    }

    public class OrderItem
    {
        public Guid OrderId { get; set;}
        public Guid ProductItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Order? Order { get; set; }
        public ProductItem? Product { get; set; }
    }
}

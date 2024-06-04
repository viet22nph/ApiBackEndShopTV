using Models.DbEntities;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DAL.Models
{
    public class Transaction:BaseEntity
    {
        public DateTime CreateAt { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // COD - MOMO - VN pay
        public string? Description { get; set; }
        public string Status { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public Guid OrderId { get; set; }
        public Order? Order { get; set; }

    }
}

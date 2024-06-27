using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Order
{
    public class TransactionDto
    {
        public decimal Amount { get; set; }
        public string Type { get; set; } // COD - MOMO - VN pay
        public string? Description { get; set; }
        public string? UserId { get; set; }
        public string Status { get; set; }
    }
}

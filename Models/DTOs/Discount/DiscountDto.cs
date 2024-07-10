using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Discount
{
    public class DiscountDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Type { get; set; } // percentage or fix-amount
        public decimal DiscountValue { get; set; }// 10000  or 10% 
        public decimal Condition { get; set; }// 200000 d
        public DateTime DateStart { get; set; }// 11/11/2024
        public DateTime DateEnd { get; set; }//11/12/2024

        public string? Description { get; set; }
        public string Status { get; set; }// pending
    }

    public class DiscountIdDto
    {
        public Guid? DiscountId { get; set; }
    }
}

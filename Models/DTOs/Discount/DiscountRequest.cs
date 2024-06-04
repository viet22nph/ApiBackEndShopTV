using Models.Status;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Discount
{
    public class DiscountRequest
    {
        [Required(ErrorMessage ="Discount code not null or empty")]
        public string Code { get; set; }
        [Required(ErrorMessage ="Type discount not null or empty")]
        public string Type { get; set; } // percentage or fix-amount
        [Required(ErrorMessage = "Discount value not null or empty")]
        [Range(0, double.MaxValue,ErrorMessage = "Discount value must be greater than or equal to 0")]
        public decimal DiscountValue { get; set; }// 10000  or 10%
        [Required(ErrorMessage = "Conditions not null or empty")]
        [Range(0, double.MaxValue, ErrorMessage = "Condition value must be greater than or equal to 0")]
        public decimal Condition { get; set; }// 200000 d // điều kiện giảm giá
        [Required(ErrorMessage = "Date start not null or empty")]
        public DateTime DateStart { get; set; }// 11/11/2024
        [Required(ErrorMessage = "Date end not null or empty")]
        public DateTime DateEnd { get; set; }//11/12/2024
        public string? Description { get; set; }
    }
}

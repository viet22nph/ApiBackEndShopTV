using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Order
{
    public class OrderUpdateStatusRequest
    {
        [Required(ErrorMessage = "Order id not null or empty")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Order status not null or empty")]
        public string Status { get; set; }
    }
}

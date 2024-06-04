using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Order
{
    public class ReviewCheckoutRequest
    {
        [Required]
        public ICollection<CheckoutItem> Items { get; set; }
    }
    public class CheckoutItem
    {
        [Required]
        public Guid ProductItemId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}

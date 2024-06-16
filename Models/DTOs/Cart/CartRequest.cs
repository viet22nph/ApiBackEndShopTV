using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Cart
{
    public class CartRequest
    {
        public string UserId { get; set; }
        public Guid ProductItemId { get; set; }
        public long? Quantity { get; set; }
        public long? IncrementBy { get; set; }

    }
}

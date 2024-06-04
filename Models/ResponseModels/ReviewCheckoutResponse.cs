using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ResponseModels
{
    public class ReviewCheckoutResponse
    {
        public ICollection<ReviewCheckoutItem> ReviewCheckoutItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public ReviewCheckoutResponse()
        {
            ReviewCheckoutItems = new List<ReviewCheckoutItem>();
        }
    }
    public class ReviewCheckoutItem
    {
        public Guid ProductItemId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal AmountDiscount { get; set; } = 0;
        public decimal Total => Quantity * Price;
    }
}

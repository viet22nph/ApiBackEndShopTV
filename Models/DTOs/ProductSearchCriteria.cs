using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class ProductSearchCriteria // de sau r lam
    {

        public Guid? CategoryId { get; set; }
        public double? MinRating { get; set; }
        public PriceSearchCriteria? price { get; set; }

        public class PriceSearchCriteria {
            public decimal? PriceMin { get; set; } = 0;
            public decimal? PriceMax { get; set; }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Product
{
    public class ProductItemDto
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public Guid? ColorId { get; set; }

        public ICollection<ProductImageDto>? ProductImages { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Product
{
    public class ProductItemUpdateRequest
    {
        public Guid? Id { get; set; }
        public int? Quantity { get; set; }
        public Guid? ColorId { get; set; }
        public ICollection<ImageProductUpdateRequest>? ProductImages { get; set; }
    }
    public class ImageProductUpdateRequest
    {
        public Guid? Id { get; set;}
        public string? Url { get; set; }
    }
}

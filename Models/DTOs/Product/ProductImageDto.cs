﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Product
{
    public class ProductImageDto
    {
        public Guid Id { get; set; }
        public Guid ProductItemId { get; set; }
        public string Url { get; set; }
    }
}
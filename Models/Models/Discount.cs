﻿using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DAL.Models
{
    public class Discount: BaseEntity
    {
        public string Code { get; set; }
        public string Type { get; set; } // percentage or fix-amount
        public decimal DiscountValue { get; set; }// 10000  or 10% 
        public decimal MinimumPurchase { get; set; }// 200000 d
        public DateTime DateStart { get; set; }// 11/11/2024
        public DateTime DateEnd { get; set; }//11/12/2024
        public string Status { get; set; }// pending
        public string? Description { get; set; } 
        public ICollection<ProductDiscount>? ProductDiscounts { get; set; }
    }

    public class ProductDiscount
    {
        public Guid DiscountId { get; set; }
        public Guid ProductId { get; set; }

        public Product? Product { get; set; }
        public Discount? Discount { get; set; }
    }
}

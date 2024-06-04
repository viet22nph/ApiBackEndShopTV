using Application.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping
{
    public class ProductDiscountMap:MappingEntityTypeConfiguration<ProductDiscount>
    {
        public override void Configure(EntityTypeBuilder<ProductDiscount> builder)
        {
            builder.ToTable("ProductDiscount");
            builder
                   .HasKey(pd => new { pd.ProductId, pd.DiscountId });
            builder
                .HasOne(pd => pd.Product)
                .WithMany(p => p.ProductDiscounts)
                .HasForeignKey(pd => pd.ProductId);

            builder
                .HasOne(pd => pd.Discount)
                .WithMany(d => d.ProductDiscounts)
                .HasForeignKey(pd => pd.DiscountId);
            base.Configure(builder);
        }
    }
}

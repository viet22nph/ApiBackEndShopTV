
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
    public class ProductItemMap:MappingEntityTypeConfiguration<ProductItem>
    {
        public override void Configure(EntityTypeBuilder<ProductItem> builder)
        {
            builder.ToTable("ProductItem");
            builder.HasKey(p=> p.Id);
            builder.Property(p=>p.ProductId)
             .IsRequired();
            builder.Property(p => p.Quantity)
             .IsRequired();
            builder
              .HasOne(pi => pi.Product)
              .WithMany(p => p.ProductItems)
              .HasForeignKey(pi => pi.ProductId);
            builder
                .HasOne(pi => pi.Color)
                .WithMany(c => c.ProductItems)
                .HasForeignKey(pi => pi.ColorId);
            builder.Property(r => r.DateCreate)
              .HasColumnType("DateTime")
              .HasDefaultValueSql("GetUtcDate()");
            base.Configure(builder);
        }
    }
}

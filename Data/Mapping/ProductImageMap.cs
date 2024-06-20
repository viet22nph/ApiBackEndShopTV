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
    public class ProductImageMap:MappingEntityTypeConfiguration<ProductImage>
    {

        public override void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.ToTable("ProductImage");
            builder.HasKey(x => x.Id);
            builder
              .HasOne(pi => pi.ProductItem)
              .WithMany(p => p.ProductImages)
              .HasForeignKey(pi => pi.ProductItemId);

            builder.Property(r => r.DateCreate)
              .HasColumnType("DateTime")
              .HasDefaultValueSql("GetUtcDate()");

            base.Configure(builder);
        }
    }
}

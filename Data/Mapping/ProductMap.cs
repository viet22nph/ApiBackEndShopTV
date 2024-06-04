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
    public class ProductMap : MappingEntityTypeConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {

            builder.ToTable("Product");
            builder.HasKey(p => p.Id);
            builder.Property(p=>p.Name).HasMaxLength(255);
            builder.Property(p=> p.ProductBrand).HasMaxLength(255);
            builder
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId);

         
            // sup

            base.Configure(builder);
        }
    }
}

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
    public class ProductSpecificationMap: MappingEntityTypeConfiguration<ProductSpecification>
    {
        public override void Configure(EntityTypeBuilder<ProductSpecification> builder)
        {
            builder.ToTable("ProductSpecification");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x=> x.SpecType)
                .HasMaxLength(255)
                .IsRequired();
            builder.Property(x => x.SpecValue)
              .IsRequired();


            base.Configure(builder);
        }
    }
}

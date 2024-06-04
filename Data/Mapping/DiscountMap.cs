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
    public class DiscountMap: MappingEntityTypeConfiguration<Discount>
    {
        public override void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.ToTable("Discount");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Code).HasMaxLength(255);
            builder.Property(d=> d.Status).HasMaxLength(30);
            builder.Property(d=> d.Type).HasMaxLength(30);

            base.Configure(builder);
        }
    }
}

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
    public class OrderItemMap: MappingEntityTypeConfiguration<OrderItem>
    {

        public override void Configure(EntityTypeBuilder<OrderItem> builder)
        {

            builder.ToTable("OrderItem");
            builder.HasKey(o => new { o.OrderId, o.ProductItemId });
            builder.Property(o => o.Quantity)
                .IsRequired();
            builder.Property(o => o.Price)
                .IsRequired();
            builder
               .HasOne(o => o.Product)
               .WithMany(p => p.OrderItems)
               .HasForeignKey(pd => pd.ProductItemId);
            builder
                .HasOne(oi=>oi.Order)
                .WithMany(o=> o.OrderItems)
                .HasForeignKey(pd => pd.OrderId);
            base.Configure(builder);
        }
    }
}

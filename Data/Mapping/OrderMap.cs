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
    public class OrderMap:MappingEntityTypeConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("tbl_Order");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.OrderType)
                .HasMaxLength(30)
                .IsRequired();
            builder.Property(o=> o.Phone)
                .HasMaxLength(11)
                .HasColumnType("char(11)");
            builder.Property(o => o.RecipientName)
                .HasMaxLength(255);
            builder.Property(o => o.Status)
                .HasMaxLength(30);

            builder.HasOne(u => u.User)
                .WithMany(o => o.Order)
                .HasForeignKey(o => o.UserId);
            builder.HasMany(o => o.Transactions)
           .WithOne(t => t.Order)
           .HasForeignKey(o => o.OrderId);
            builder.Property(r => r.DateCreate)
              .HasColumnType("DateTime")
              .HasDefaultValueSql("GetUtcDate()");
            base.Configure(builder);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping
{
    public class ReviewMap:MappingEntityTypeConfiguration<Review>
    {
        public override void Configure(EntityTypeBuilder<Review> builder)
        {

            builder.ToTable("Review");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.ProductId)
                .IsRequired();
            builder.Property(r => r.UserId)
                .IsRequired();
            builder.Property(r => r.CreateAt)
                .HasColumnType("DateTime")
                .HasDefaultValueSql("GetUtcDate()");
            builder.HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);
            builder.HasOne(r => r.Product)
              .WithMany(u => u.Reviews)
              .HasForeignKey(r => r.ProductId);

            base.Configure(builder);
        }
    }
}

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
    public class CategoryMap: MappingEntityTypeConfiguration<Category>
    {

        public override void Configure(EntityTypeBuilder<Category> builder)
        {

            builder.ToTable("Category");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(255).IsRequired();    
            builder.Property(x=> x.NomalizedName).HasMaxLength(255);
            builder.HasOne(c => c.CategoryParentNavigation)
              .WithMany(c => c.CategoryChildren)
              .HasForeignKey(c => c.CategoryParent);
            builder.HasMany(c => c.Products).WithOne(c => c.Category).HasForeignKey(c => c.CategoryId);
            builder.Property(r => r.DateCreate)
              .HasColumnType("DateTime")
              .HasDefaultValueSql("GetUtcDate()");
            base.Configure(builder);
        }

    }
}

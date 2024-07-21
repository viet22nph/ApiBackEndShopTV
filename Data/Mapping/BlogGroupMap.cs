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
    public class BlogGroupMap : MappingEntityTypeConfiguration<BlogGroup>
    {
        public override void Configure(EntityTypeBuilder<BlogGroup> builder)
        {

            builder.ToTable("BlogGroup");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(30).IsRequired();
            builder.HasIndex(x => x.Name).IsUnique();
            builder.HasMany(x => x.Blogs).WithOne(y => y.BlogGroup).HasForeignKey(x => x.BlogGroupId);
            builder.Property(r => r.DateCreate)
           .HasColumnType("DateTime")
           .HasDefaultValueSql("GetUtcDate()");
            base.Configure(builder);
        }
    }
}

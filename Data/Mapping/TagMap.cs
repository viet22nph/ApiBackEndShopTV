using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;

namespace Data.Mapping
{
    public class TagMap : MappingEntityTypeConfiguration<Tag>
    {
        public override void Configure(EntityTypeBuilder<Tag> builder)
        {

            builder.ToTable("Tag");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.TagTitle).HasMaxLength(300).IsRequired();
            builder.HasIndex(x => x.TagTitle)
               .IsUnique();
            builder.HasMany(x => x.TagBlogs).WithOne(y => y.Tag).HasForeignKey(x => x.TagId);
            builder.Property(r => r.DateCreate)
           .HasColumnType("DateTime")
           .HasDefaultValueSql("GetUtcDate()");
            base.Configure(builder);
        }
    }
}

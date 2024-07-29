using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;
namespace Data.Mapping
{
    public class BlogMap : MappingEntityTypeConfiguration<Blog>
    {
        public override void Configure(EntityTypeBuilder<Blog> builder)
        {

            builder.ToTable("Blog");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(300).IsRequired();
            builder.HasIndex(x => x.Title).IsUnique();
            builder.Property(x=> x.Content).IsRequired();
            builder.Property(x=> x.BlogImage).IsRequired().HasMaxLength(500);
            builder.HasMany(x => x.TagBlogs).WithOne(y => y.Blog).HasForeignKey(x => x.BlogId);
            builder.HasOne(x=> x.Author).WithMany(y=> y.Blogs).HasForeignKey(x => x.AuthorId);
            builder.Property(x=> x.Slug).IsRequired();
            builder.HasIndex(x => x.Slug).IsUnique();
            builder.Property(r => r.DateCreate)
           .HasColumnType("DateTime")
           .HasDefaultValueSql("GetUtcDate()");
            base.Configure(builder);
        }
    }
}

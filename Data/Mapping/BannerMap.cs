
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;
namespace Data.Mapping
{
    public class BannerMap : MappingEntityTypeConfiguration<Banner>
    {
        public override void Configure(EntityTypeBuilder<Banner> builder)
        {

            builder.ToTable("Banner");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Url).HasMaxLength(120).IsRequired();
            builder.Property(r => r.DateCreate)
           .HasColumnType("DateTime")
           .HasDefaultValueSql("GetUtcDate()");
            base.Configure(builder);
        }
    }
}

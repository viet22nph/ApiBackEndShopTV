
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;
namespace Data.Mapping
{
    public class GroupBannerMap : MappingEntityTypeConfiguration<GroupBanner>
    {
        public override void Configure(EntityTypeBuilder<GroupBanner> builder)
        {

            builder.ToTable("GroupBanner");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.GroupName).HasMaxLength(30).IsRequired();
            builder.HasMany(x=> x.Banners).WithOne(y=> y.Group).HasForeignKey(x=> x.GroupId);
            builder.Property(r => r.DateCreate)
              .HasColumnType("DateTime")
              .HasDefaultValueSql("GetUtcDate()");
            base.Configure(builder);
        }
    }
}

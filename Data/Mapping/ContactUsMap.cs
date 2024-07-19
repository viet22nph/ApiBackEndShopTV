using Application.DAL.Models;
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
    public class ContactUsMap : MappingEntityTypeConfiguration<ContactUs>
    {
        public override void Configure(EntityTypeBuilder<ContactUs> builder)
        {

            builder.ToTable("ContactUs");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Email).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Phone).HasMaxLength(11).IsRequired();
            builder.Property(x=> x.ContactContent).HasMaxLength(1000).IsRequired();
            builder.Property(r => r.DateCreate)
              .HasColumnType("DateTime")
              .HasDefaultValueSql("GetUtcDate()");
            base.Configure(builder);
        }
    }
}

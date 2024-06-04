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
    public class ColorMap:MappingEntityTypeConfiguration<Color>
    {
        public override void Configure(EntityTypeBuilder<Color> builder)
        {
            builder.ToTable("Color");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ColorCode)
                .HasMaxLength(30)
                .IsRequired();
            builder.Property(x => x.ColorName)
                .HasMaxLength(255)
                .IsRequired();


            base.Configure(builder);
        }


    }
}

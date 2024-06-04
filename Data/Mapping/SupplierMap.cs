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
    public class SupplierMap: MappingEntityTypeConfiguration<Supplier>
    {

        public override void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Supplier");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.SupplierName)
                .HasMaxLength(255)
                .IsRequired();
            builder.Property(s => s.ContactPerson)
                .HasMaxLength(255)
                .IsRequired();
            builder.Property(s => s.ContactPhone)
                .HasMaxLength(11)
                .HasColumnType("char(11)");



            base.Configure(builder);
        }
    }
}

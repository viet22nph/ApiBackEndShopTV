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
    public class TransactionMap: MappingEntityTypeConfiguration<Transaction>
    {
        public override void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transaction");
            builder.HasKey(t => t.Id);
            builder.Property(t=> t.Type)
                .HasMaxLength(30)
                .IsRequired();
            builder.Property(t => t.Status)
                .HasMaxLength(30)
                .IsRequired();
            builder.Property(r => r.DateCreate)
            .HasColumnType("DateTime")
            .HasDefaultValueSql("GetUtcDate()");
            base.Configure(builder);
        }
    }
}

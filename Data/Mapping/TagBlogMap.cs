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
    public class TagBlogMap : MappingEntityTypeConfiguration<TagBlog>
    {
        public override void Configure(EntityTypeBuilder<TagBlog> builder)
        {

            builder.ToTable("TagBlog");
            builder.HasKey(x => new { x.TagId, x.BlogId });
            base.Configure(builder);
        }
    }
}

using Slugify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public static class Helper
    {
        public static string CreateSlug(string value)
        {
            var slugHelper = new SlugHelper();
            var slug = slugHelper.GenerateSlug(value);
            return slug;
        }
    }
}

using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.BlogGroupRepo
{
    public class BlogGroupRepository : IBlogGroupRepository
    {
        private readonly ApplicationDbContext  _context;

        public BlogGroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<bool> CheckNameExistsAsync(string name)
        {
            return await _context.Set<BlogGroup>().AnyAsync(x => x.Name == name);
        }
        public async Task<BlogGroup> GetBlogGroupDetailAsync(Guid id)
        {
            return await _context.Set<BlogGroup>()
                .Include(b => b.Blogs)
                             .ThenInclude(b => b.TagBlogs)
                                 .ThenInclude(tb => tb.Tag)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

    }
}

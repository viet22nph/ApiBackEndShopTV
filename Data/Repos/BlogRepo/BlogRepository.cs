using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.BlogRepo
{
    public class BlogRepository: IBlogRepository
    {
        private readonly ApplicationDbContext _context;

        public BlogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(ICollection<Blog>, int)> GetBlogsAsync(int pageNumber, int pageSize)
        {
            int count = await _context.Set<Blog>().CountAsync();
            var data = await _context.Set<Blog>().
                Include(b => b.Author)
                .Include(b => b.BlogGroup)
                .Include(b => b.TagBlogs)
                .ThenInclude(tb => tb.Tag)
                .OrderByDescending(x => x.DateCreate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return(data, count);
        }
        public async Task<Blog> GetBlogByIdAsync(Guid id)
        {
            return await _context.Set<Blog>().
                Include(b => b.Author)
                .Include(b => b.BlogGroup)
                .Include(b => b.TagBlogs)
                .ThenInclude(tb => tb.Tag)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<(ICollection<Blog>, int)> GetBlogByGroupIdAsync(Guid id, int pageNumber, int pageSize)
        {
            int count = await _context.Set<Blog>().CountAsync();
            var data = await _context.Set<Blog>().
                Include(b => b.Author)
                .Include(b => b.BlogGroup)
                .Include(b => b.TagBlogs)
                .ThenInclude(tb => tb.Tag)
                .Where(b => b.BlogGroupId == id)
                .OrderByDescending(x => x.DateCreate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (data, count);
        }

        public async Task<(ICollection<Blog>, int)> GetBlogsByTagIdAsync(Guid id, int pageNumber, int pageSize)
        {
            var totalBlogs = await _context.Set<TagBlog>()
               .CountAsync(tb => tb.TagId == id);
            var data = await _context.Set<Blog>()
        .Include(b => b.Author)
        .Include(b => b.BlogGroup)
        .Include(b => b.TagBlogs)
        .ThenInclude(tb => tb.Tag)
        .Where(b => b.TagBlogs.Any(tb => tb.TagId == id))
        .OrderByDescending(x => x.DateCreate)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
            return (data, totalBlogs);
        }
         
    }
}

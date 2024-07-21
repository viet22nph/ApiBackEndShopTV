using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.BlogRepo
{
    public interface IBlogRepository
    {
        Task<(ICollection<Blog>, int)> GetBlogsAsync(int pageNumber, int pageSize);
        Task<Blog> GetBlogByIdAsync(Guid id);
        Task<(ICollection<Blog>, int)> GetBlogsByTagIdAsync(Guid id, int pageNumber, int pageSize);
        Task<(ICollection<Blog>, int)> GetBlogByGroupIdAsync(Guid id, int pageNumber, int pageSize);
    }
}

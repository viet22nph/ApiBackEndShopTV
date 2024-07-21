using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.BlogGroupRepo
{
    public interface IBlogGroupRepository
    {
        Task<bool> CheckNameExistsAsync(string name);
         Task<BlogGroup> GetBlogGroupDetailAsync(Guid id);
    }
}

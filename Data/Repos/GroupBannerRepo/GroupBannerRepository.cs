using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.GroupBannerRepo
{
    public class GroupBannerRepository : IGroupBannerRepository
    {
        // context
        private readonly ApplicationDbContext _context;
        public GroupBannerRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<GroupBanner> GetDetailGroupBannerAsync(Guid id)
            => await _context.Set<GroupBanner>().Include(gb => gb.Banners).FirstOrDefaultAsync(gb => gb.Id == id);
    }
}

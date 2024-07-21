using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.BannerRepo
{
    public class BannerRepository : IBannerRepository
    {
        private readonly ApplicationDbContext _context;
        public BannerRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ICollection<Banner>> GetBannersAsync(int pageNumber, int pageSize)
        {
            var banners = await _context.Set<Banner>()
            .Include(b => b.Group)
            .OrderBy(b => b.GroupId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            return banners;
        } 
    }
}

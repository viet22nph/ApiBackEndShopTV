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
        public async Task<ICollection<Banner>> GetBanners()
        {
            return await _context.Set<Banner>().Include(b => b.Group).GroupBy(x => x.GroupId).Select(group => group.FirstOrDefault()).ToListAsync();
        } 
    }
}

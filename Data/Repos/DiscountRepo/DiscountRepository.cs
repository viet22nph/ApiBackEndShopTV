using Application.DAL.Models;
using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.DiscountRepo
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly ApplicationDbContext _context;

        public DiscountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<Discount>().CountAsync();
        }

        public async Task<ICollection<Discount>> GetDiscounts(int pageNumber, int pageSize)
        {
            var discounts = await _context.Set<Discount>()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            return discounts;
        }
    }
}

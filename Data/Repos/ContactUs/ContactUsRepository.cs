using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.ContactUs
{
    public class ContactUsRepository :IContactUsRepository
    {
        private readonly ApplicationDbContext _context;
        public ContactUsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(ICollection<Models.Models.ContactUs>, int)> GetContactUsAsync(int pageSize, int pageNumber)
        {
            int count = await _context.Set<Models.Models.ContactUs>().CountAsync();
            var contacts = await _context
                .Set<Models.Models.ContactUs>()
                .OrderByDescending(x => x.DateCreate)
                .Take(pageSize)
                .Skip((pageNumber-1)* pageSize)
                .ToListAsync();
            return (contacts, count);
        }
    }
}

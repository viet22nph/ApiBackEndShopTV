using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.UserRepo
{
    public class UserRepository:IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<ApplicationUser>> GetUsers(int pageNumber, int pageSize)
        {
            var users = await _context.Users.Skip((pageNumber - 1)*pageSize).Take(pageSize).ToListAsync();
            return users;
        }
        public async Task<ApplicationUser> GetUserById(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}

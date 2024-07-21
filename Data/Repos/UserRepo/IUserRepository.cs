using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.UserRepo
{
    public interface IUserRepository
    {
        Task <ICollection<ApplicationUser>> GetUsers(int pageNumber, int pageSize);
        Task<ApplicationUser> GetUserById(string id);
    }
}

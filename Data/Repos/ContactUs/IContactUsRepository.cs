using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.ContactUs
{
    public interface IContactUsRepository
    {
        Task<(ICollection<Models.Models.ContactUs>, int)> GetContactUsAsync(int pageSize, int pageNumber);
    }
}

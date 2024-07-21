using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.BannerRepo
{
    public interface IBannerRepository
    {
        Task<ICollection<Banner>> GetBannersAsync(int pageNumber, int pageSize);
    }
}

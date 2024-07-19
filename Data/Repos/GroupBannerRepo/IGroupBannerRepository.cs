using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.GroupBannerRepo
{
    public interface IGroupBannerRepository
    {
        Task<GroupBanner> GetDetailGroupBannerAsync(Guid id);


    }
}

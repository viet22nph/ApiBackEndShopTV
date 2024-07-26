using Models.DTOs.GroupBanner;
using Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IGroupBannerService
    {
        Task<BaseResponse<ICollection<GroupBannerDto>>> GetGroupBannerAsync();
        Task<BaseResponse<GroupBannerDetailDto>> GetDetailGroupBannerAsync(Guid id);
        Task<BaseResponse<GroupBannerDto>> ToogleEnableAsync(Guid id);
        Task<BaseResponse<GroupBannerDetailDto>> GetDetailGroupBannerByNameAsync(string name);
    }
}

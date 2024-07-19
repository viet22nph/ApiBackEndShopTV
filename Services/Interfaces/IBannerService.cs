using Models.DTOs.Banner;
using Models.DTOs.Banner.Request;
using Models.ResponseModels;

namespace Services.Interfaces
{
    public interface IBannerService
    {
        Task<BaseResponse<BannerDto>> CreateBannerServiceAsync(BannerRequestDto payload);
        Task<BaseResponse<BannerDto>> ToogleEnableAsync(Guid id);

        Task<BaseResponse<ICollection<BannerDto>>> GetBannersAsync();
        Task<bool> RemoveBanner(Guid id);
    }
}

using Models.DTOs.BlogGroup.Request;
using Models.DTOs.BlogGroup;
using Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBlogGroupService
    {
        Task<BaseResponse<BlogGroupDto>> CreateBlogGroupAsync(BlogGroupRequestDto payload);
        Task<BaseResponse<ICollection<BlogGroupDto>>> GetBlogGroupAsync();
        Task<BaseResponse<BlogGroupDto>> UpdateBlogGroupAsync(UpdateBlogGroupRequestDto payload);
        Task<bool> RemoveBlogGroupAsync(Guid blogGroupId);
        Task<BaseResponse<BlogGroupDetailDto>> GetBlogGroupDetailAsync(Guid blogGroupId);
    }
}

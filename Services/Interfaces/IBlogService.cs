using Models.DTOs.Blog.Request;
using Models.DTOs.Blog;
using Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBlogService
    {
        Task<BaseResponse<BlogDto>> CreateBlogAsync(BlogRequestDto payload);
        Task<bool> RemoveBlogAsync(Guid id);
        Task<(BaseResponse<ICollection<BlogDto>>, int)> GetBlogAsync(int pageNumber, int pageSize);
        Task<BaseResponse<BlogDto>> UpdateBlogAsync(UpdateBlogRequestDto payload);
        Task<(BaseResponse<ICollection<BlogDto>>, int)> GetBlogByGroupIdAsync(Guid id, int pageNumber, int pageSize);
        Task<(BaseResponse<ICollection<BlogDto>>, int)> GetBlogByTagIdAsync(Guid id, int pageNumber, int pageSize);
    }
}

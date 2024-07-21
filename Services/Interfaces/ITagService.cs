using Models.DTOs.Tag;
using Models.DTOs.Tag.request;
using Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ITagService
    {
        Task<BaseResponse<TagDto>> CreateTagAsync(TagRequsetDto payload);
        Task<BaseResponse<ICollection<TagDto>>> GetTagAsync();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DAL.Models;
using Models.DTOs.Category;
using Models.ResponseModels;
namespace Services.Interfaces
{
    public interface ICategoryService
    {
        Task<BaseResponse<List<CategoryDto>>> GetCategories();
        Task<BaseResponse<CategoryDto>> GetCategory(Guid id);
        Task<BaseResponse<CategoryDto>> InsertCategory(CategoryRequest request);
        Task<BaseResponse<CategoryDto>> UpdateCategory(Guid id,CategoryUpdateRequest request);
        Task<BaseResponse<string>> DeleteCategory(Guid id);

    }
}

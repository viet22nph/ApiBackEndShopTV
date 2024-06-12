using Application.DAL.Models;
using Microsoft.AspNetCore.Http;
using Models.DTOs.Product;
using Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUploadPhotoService
    {

        Task<BaseResponse<ICollection<UploadImageResponse>>> UploadPhotoProduct(ICollection<IFormFile> files);
        Task<BaseResponse<string>> RemovePhotoProduct(Guid id);
    }
}

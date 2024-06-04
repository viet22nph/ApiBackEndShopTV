﻿using Application.DAL.Models;
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

        Task<BaseResponse<ProductImageDto>> UploadPhotoProduct(IFormFile file, Guid id);
        Task<BaseResponse<string>> RemovePhotoProduct(Guid id);
    }
}

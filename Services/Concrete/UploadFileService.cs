using Application.DAL.Models;
using AutoMapper;
using CloudinaryDotNet;
using Core.Exceptions;
using Core.Interfaces;
using Core.Services;
using Data.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Models.DTOs.Product;
using Models.ResponseModels;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete
{
    public class UploadFileService : IUploadPhotoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUploadPhotoCoreService _uploadPhotoCoreService;
        private readonly IMapper _mapper;

        public UploadFileService(IUnitOfWork unitOfWork, IUploadPhotoCoreService uploadPhotoCoreService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _uploadPhotoCoreService = uploadPhotoCoreService;
            _mapper = mapper;
        }

        public async Task<BaseResponse<string>> RemovePhotoProduct(Guid id)
        {
            try
            {
                var image = await _unitOfWork.Repository<ProductImage>().GetById(id);
                if (image == null)
                {
                    throw new ApiException("Internal server error: Not found image") { StatusCode = (int)HttpStatusCode.NotFound };
                }

                int result = await _unitOfWork.Repository<ProductImage>().Delete(image);
                if(result <= 0)
                {
                    throw new ApiException("Internal server error: RemoveImage Error") { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                return new BaseResponse<string>("Remove success");
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }

        public async Task<BaseResponse<ICollection<UploadImageResponse>>> UploadPhotoProduct(ICollection<IFormFile> files)
        {
            try
            {
                var listImageResponse = new List<UploadImageResponse>();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                foreach (IFormFile fileItem in files)
                {
                    // Optional: Check the file extension
                    var extension = Path.GetExtension(fileItem.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                    {
                        throw new ApiException("Invalid file extension. Only .jpg, .jpeg, .png, .gif, .bmp are allowed.")
                        {
                            StatusCode = (int)HttpStatusCode.BadRequest
                        };
                    }
                    var uploadResult = await _uploadPhotoCoreService.UploadImage(fileItem, false); // Assuming false for non-profile images

                    if (uploadResult == null || string.IsNullOrEmpty(uploadResult.Url.ToString()))
                    {
                        throw new ApiException("Photo upload failed") { StatusCode = (int)HttpStatusCode.BadRequest };
                    }
                      // Optional: Check the file extension
         
                    listImageResponse.Add(new UploadImageResponse { Url = uploadResult.Url.ToString() });
                }
                return new BaseResponse<ICollection<UploadImageResponse>> (listImageResponse, "Photo upload success" );

            }
            catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }
    }
}

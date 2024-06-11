using Application.DAL.Models;
using AutoMapper;
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
                    throw new ApiException("Not found image") { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                var result = await _uploadPhotoCoreService.DeleteImage(image.CloudPublic);
                return new BaseResponse<string>("Remove success");
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }

        public async Task<BaseResponse<ICollection<ProductImageDto>>> UploadPhotoProduct(ICollection<IFormFile> files, Guid id)
        {
            try
            {
                var productItem = await _unitOfWork.Repository<ProductItem>().GetById(id);
                if (productItem == null)
                {
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }
                List<ProductImage> productImages = new List<ProductImage>();
                foreach (IFormFile fileItem in files)
                {
                    var uploadResult = await _uploadPhotoCoreService.UploadImage(fileItem, false); // Assuming false for non-profile images

                    if (uploadResult == null || string.IsNullOrEmpty(uploadResult.Url.ToString()))
                    {
                        throw new ApiException("Photo upload failed") { StatusCode = (int)HttpStatusCode.BadRequest };
                    }
                  
                    var productImage = new ProductImage
                    {
                        ProductItemId = id,
                        Url = uploadResult.Url.ToString(),
                        CloudPublic = uploadResult.PublicId

                    };
                    productImage = await _unitOfWork.Repository<ProductImage>().Insert(productImage);
                    productImages.Add(productImage);
                }
               

               

              
                var res = _mapper.Map<List<ProductImageDto>>(productImages);
                return new BaseResponse<ICollection<ProductImageDto>> (res, "Photo upload success" );

            }
            catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }
    }
}

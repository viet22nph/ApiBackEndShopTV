using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class UploadPhotoCoreService: IUploadPhotoCoreService
    {
        private Cloudinary _cloudinary;

        public UploadPhotoCoreService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<ImageUploadResult> UploadImage(IFormFile formFile, bool profile)
        {
            var uploadResult = new ImageUploadResult();
            var transformation = profile
                ? new Transformation().Width(500).Height(500).Crop("fill").Gravity(Gravity.Face)
                : new Transformation().Height(512).Crop("fit");
            if (formFile.Length > 0)
            {
                await using var stream = formFile.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(formFile.FileName, stream),
                    Transformation = transformation,
                    Folder = "NDV_Images"
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeleteImage(string publicId)
        {
            if (string.IsNullOrEmpty(publicId)) return null;
            var deleteParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}

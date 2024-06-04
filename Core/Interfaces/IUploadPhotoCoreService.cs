using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
namespace Core.Interfaces
{
    public interface IUploadPhotoCoreService
    {
        Task<ImageUploadResult> UploadImage(IFormFile formFile, bool profile);
        Task<DeletionResult> DeleteImage(string publicId);
    }
}

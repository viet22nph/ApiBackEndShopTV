using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.UploadPhoto;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadPhotoController : ControllerBase
    {
        private readonly IUploadPhotoService _uploadPhoto;

        public UploadPhotoController(IUploadPhotoService uploadPhoto)
        {
            _uploadPhoto = uploadPhoto;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadProductImage([FromForm] UploadProductItemImageRequest request)
        {
            if (request.File == null || request.File.Count == 0)
            {
                return BadRequest("No file uploaded.");
            }
            var result = await _uploadPhoto.UploadPhotoProduct(request.File, request.ProductItemId);
            return Ok(result);
        }

        [HttpDelete("remove-image-product-item/{id}")]
        public async Task<IActionResult> RemoveProductImage(Guid id)
        {
            var result = await _uploadPhoto.RemovePhotoProduct(id);
            return Ok(result);
        }
    }
}

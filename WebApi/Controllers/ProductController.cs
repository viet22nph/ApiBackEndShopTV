using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Product;
using Services.Interfaces;
using System.Net.WebSockets;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductController : ControllerBase
    {

        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpPost("publish")]
        public async Task<IActionResult> UpdatePublish([FromBody] Guid id)
        {
            if (id == null)
            {
                return BadRequest(new { message = "Id not null or empty" });
            }

            var result = await _productService.UpdateProductPublish(id);
            if (result.Errors == null || !result.Errors.Any())
            {
            }
            return Ok(result);

        }

        [HttpPost("draft")]
        public async Task<IActionResult> UpdateDraft([FromBody] Guid id)
        {
            if (id == null)
            {
                return BadRequest(new { message = "Id not null or empty" });
            }

            var result = await _productService.UpdateProductDraft(id);
            if (result.Errors == null || !result.Errors.Any())
            {
            }
            return Ok(result);

        }
        [HttpPost("list")]
        public async Task<IActionResult> GetProducts(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _productService.GetProducts(pageNumber, pageSize);
            if (result.Errors == null || !result.Errors.Any())
            {
            }
            return Ok(result);
        }

        [HttpPost("list-draft")]
        public async Task<IActionResult> GetProductsIsDraft(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _productService.GetProductsIsDraft(pageNumber, pageSize);
            if (result.Errors == null || !result.Errors.Any())
            {
            }
            return Ok(result);
        }
        [HttpPost("list-publish")]
        public async Task<IActionResult> GetProductsIsPublish(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _productService.GetProductsIsPublish(pageNumber, pageSize);
            if (result.Errors == null || !result.Errors.Any())
            {
            }
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("{id}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            if (id == null)
            {
                return BadRequest("Id not null or empty");
            }
            var result = await _productService.GetProduct(id);
            return Ok(result);
        }

        [Authorize(Policy = "canCreateProduct")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productService.CreateProduct(request);
            if (result.Errors == null || !result.Errors.Any())
            {
            }
            return Ok(result);
        }

        [Authorize(Policy = "canEditProduct")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductUpdateRequest request)
        {
            if(id == null)
            {

                return BadRequest("Id not null or empty");
            }
            var result = await _productService.UpdateProduct(id,request);
            if (result.Errors == null || !result.Errors.Any())
            {
            }
            return Ok(result);
        }
    }
}

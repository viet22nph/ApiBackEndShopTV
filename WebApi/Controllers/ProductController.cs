using Caching;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Cart;
using Models.DTOs.Discount;
using Models.DTOs.Product;
using Models.Settings;
using Services.Interfaces;
using System.Net.WebSockets;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class ProductController : ControllerBase
    {
        private readonly ICacheManager _cacheManager;
        private readonly IProductService _productService;

        public ProductController(IProductService productService, ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            _productService = productService;
        }
        [HttpPost("publish")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Product.Update)]
        public async Task<IActionResult> UpdatePublish([FromBody] ProductIdDto request)
        {
            if (request?.ProductId == null)
            {
                return BadRequest(new { message = "Id not null or empty" });
            }

            var result = await _productService.UpdateProductPublish(request.ProductId);
            _cacheManager.RemoveByPrefix("api/Product");
            return Ok(result);

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Product.Update)]
        [HttpPost("draft")]
        public async Task<IActionResult> UpdateDraft([FromBody] ProductIdDto request)
        {
            if (request?.ProductId == null)
            {
                return BadRequest(new { message = "Id not null or empty" });
            }

            var result = await _productService.UpdateProductDraft(request.ProductId);
            _cacheManager.RemoveByPrefix("api/Product");
            return Ok(result);

        }
    
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Product.Update)]
        [HttpPost("list")]
        [Cache]
        public async Task<IActionResult> GetProducts(int pageNumber = 1, int pageSize = 10)
        {
            var (result, total) = await _productService.GetProducts(pageNumber, pageSize);

            return Ok(new {
                message = result.Message,
                data = result.Data,
                pageNumber = pageNumber,
                pageSize = pageSize,
                total = total
            });

        }

        [HttpPost("list-draft")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Product.Update)]
        [Cache]

        public async Task<IActionResult> GetProductsIsDraft(int pageNumber = 1, int pageSize = 10)
        {
            var (result, total) = await _productService.GetProductsIsDraft(pageNumber, pageSize);

            return Ok(new
            {
                message = result.Message,
                data = result.Data,
                pageNumber = pageNumber,
                pageSize = pageSize,
                total = total
            });
        }
        [HttpPost("list-publish")]
        [Cache]
        public async Task<IActionResult> GetProductsIsPublish(int pageNumber = 1, int pageSize = 10)
        {
            var (result, total) = await _productService.GetProductsIsPublish(pageNumber, pageSize);

            return Ok(new
            {
                message =result.Message,
                data = result.Data,
                pageNumber = pageNumber,
                pageSize = pageSize,
                total = total
            });
        }



        [HttpPost("list-publish-to-price")]
        [Cache]
        public async Task<IActionResult> GetProductsIsPublishPrice(decimal fromPrice, decimal toPrice, int pageNumber = 1, int pageSize = 10)
        {
            var (result, total) = await _productService.GetProductsIsPublishByPrice(fromPrice, toPrice, pageNumber, pageSize);

            return Ok(new
            {
                message = result.Message,
                data = result.Data,
                pageNumber = pageNumber,
                pageSize = pageSize,
                total = total
            });
        }
            [HttpPost("products-of-the-same-category/{id}")]
            [Cache]
            public async Task<IActionResult> GetProductsOfTheSameType(Guid id, int pageNumber =1, int pageSize=10)
            {
                var (result, total) = await _productService.GetProductsOfTheSameCategoryAsync(id,pageNumber, pageSize);

                return Ok(new
                {
                    message = result.Message,
                    data = result.Data,
                    pageNumber = pageNumber,
                    pageSize = pageSize,
                    total = total
                });
            }
            [Cache(1)]
        [AllowAnonymous]
        [HttpPost("{id}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            if (id == null)
            {
                return BadRequest("Id not null or empty");
            }
            var result = await _productService.GetProduct(id);
            await _cacheManager.IncrementProductViewCountAsync(id);
            return Ok(result);
        }

        
        [HttpPost("create")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Product.Create)]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productService.CreateProduct(request);
            _cacheManager.RemoveByPrefix("api/Product");
            return Ok(result);
        }

       
        [HttpPatch("{id}")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Product.Update)]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductUpdateRequest request)
        {
            if(id == null)
            {

                return BadRequest("Id not null or empty");
            }
            var result = await _productService.UpdateProduct(id,request);
            _cacheManager.RemoveByPrefix("api/Product");
            return Ok(result);
        }
        [HttpPost("add-image/{id}")]

        public async Task<IActionResult> AddImage(Guid id, [FromBody] string url)
        {
            if(string.IsNullOrWhiteSpace(url))
            {
                return BadRequest(new { message = "url is requied" });
            }
            await _productService.AddImage(id, url);
            _cacheManager.RemoveByPrefix("api/Product");
            return NoContent();
        }
        [HttpPost("top-best-selling-product")]
        public async Task<IActionResult> GetTopBestSellingProduct([FromBody] TopSellRequest request)
        {
            if(request.Top <= 0)
            {
                return BadRequest(new
                {
                    message = "The input value cannot be less than or equal to 0"
                });
            }    
            var result = await _productService.GetTopBestSellingProducts(request.Top, request.StartDate, request.EndDate);
            return Ok(result);
        }

        [HttpPost("new-product")]
        public async Task<IActionResult> GetNewProducts(int offset =1, int limit = 10)
        {
            if(offset <= 0) {
                offset = 1;
            }
            if(limit <= 0)
            {
                limit = 10;
            }

            var (result, count) =await _productService.GetNewProducts(limit, offset);
                
            return Ok(new
            {
                message = result.Message,
                data = result.Data,
                offset = offset,
                limit = limit,
                count = count
            });

        }

        [HttpPost("get-product-by-category-publish/{categoryId}")]
        public async Task<IActionResult> GetProductByCategoryPublish(Guid categoryId, int offset =1, int limit=10)
        {
            var (result, count) = await _productService.GetProductByCategoryPublish(categoryId, limit, offset);
            return Ok(new
            {
                message = result.Message,
                data = result.Data,
                offset = offset,
                limit = limit,
                count = count
            });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Product.Read)]
        [HttpPost("get-product-by-category/{categoryId}")]
        public async Task<IActionResult> GetProductByCategory(Guid categoryId, int offset = 1, int limit = 10)
        {
            var (result, count) = await _productService.GetProductByCategory(categoryId, limit, offset);
            return Ok(new
            {
                message = result.Message,
                data = result.Data,
                offset = offset,
                limit = limit,
                count = count
            });
        }
        [Cache]
        [HttpPost("query-product")]
        public async Task<IActionResult> QueryProduct([FromQuery] string? query, int offset =1, int limit = 10)
        {
            if(query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            var (data, count) = await _productService.QueryProduct(query, offset, limit);
            return Ok(new
            {
                message = "Products",
                data = data,
                count = count,
                limit = limit,
                offset = offset
            });
        }
        //[HttpPost("remove-product-item/{id}")]

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Policy = Permissions.Product.Update)]
        //public async Task<IActionResult> RemoveProductItem(Guid id)
        //{
        //     await _productService.RemoveProductItem(id);

        //    _cacheManager.RemoveByPrefix("api/Product");
        //    return NoContent();
        //}
        //[HttpPost("remove-product-specification/{id}")]
        //public async Task<IActionResult> RemoveProductSpec(Guid id)
        //{
        //    await _productService.RemoveProductSpecification(id);

        //    _cacheManager.RemoveByPrefix("api/Product");
        //    return NoContent();
        //}

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Product.Update)]
        [HttpPost("update-discount/{productId}")]
        public async Task<IActionResult> UpdateDiscount(Guid productId, [FromBody] DiscountIdDto request)
        {
            var rs = await _productService.updateProductDiscount(productId, request.DiscountId);
            _cacheManager.RemoveByPrefix("api/Product");
            return Ok(rs);
        }
        [HttpPut("remove-discount-product")]
        public async Task<IActionResult> RemoveDiscountProduct(Guid id)
        {
            var rs = await _productService.RemoveDiscountProduct(id);
            return Ok(rs);
        }
    }

    public class TopSellRequest
    {
        public int Top { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-1);
        public DateTime EndDate { get; set; } = DateTime.Today;
    }

}

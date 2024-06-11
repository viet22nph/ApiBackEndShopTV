﻿using Caching;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Product;
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
        public async Task<IActionResult> UpdatePublish([FromBody] Guid id)
        {
            if (id == null)
            {
                return BadRequest(new { message = "Id not null or empty" });
            }

            var result = await _productService.UpdateProductPublish(id);
            _cacheManager.RemoveByPrefix("api/Product");
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
            _cacheManager.RemoveByPrefix("api/Product");
            return Ok(result);

        }
        [HttpPost("list")]
        [Cache]
        public async Task<IActionResult> GetProducts(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _productService.GetProducts(pageNumber, pageSize);
           
            return Ok(result);
        }

        [HttpPost("list-draft")]
        [Cache]
        public async Task<IActionResult> GetProductsIsDraft(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _productService.GetProductsIsDraft(pageNumber, pageSize);
           
            return Ok(result);
        }
        [HttpPost("list-publish")]
        [Cache]
        public async Task<IActionResult> GetProductsIsPublish(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _productService.GetProductsIsPublish(pageNumber, pageSize);
         
            return Ok(result);
        }
        [Cache]
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

        
        [HttpPost("create")]
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
    }
}

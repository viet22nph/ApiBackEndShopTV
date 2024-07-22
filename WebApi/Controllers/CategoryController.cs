using AutoMapper;
using Caching;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Category;
using Models.Settings;
using Services.Interfaces;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        private readonly ICategoryService _category;
        private readonly IMapper _mapper;
        private readonly ICacheManager _cacheManager;
        public CategoryController(ICategoryService category, 
            IMapper mapper,
            ICacheManager cacheManager)
        {
            _category = category;
            _mapper = mapper;
            _cacheManager = cacheManager;
        }

        [HttpPost("list")]
        [Cache]
        public async Task<IActionResult> GetCatogories()
        {
            var result = await _category.GetCategories();
           
            return Ok(result);

        }
        [Cache]
        [HttpPost("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Category.Read)]
        public async Task<IActionResult> GetCatogories(Guid id)
        {
            var result = await _category.GetCategory(id);
           
            // xóa cache

            return Ok(result);

        }
        [HttpPost("insert")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Category.Create)]
        public async Task<IActionResult> GetCatogories(CategoryRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }    
            var result = await _category.InsertCategory(request);
         
            _cacheManager.RemoveByPrefix("api/Category");
            return Ok(result);

        }
        [HttpPut("update/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Category.Update)]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryUpdateRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(id == null)
            {
                return BadRequest(new { message = "Id not null or empty" });
            }
            var result = await _category.UpdateCategory(id, request);
            
            _cacheManager.RemoveByPrefix("api/Category");
            return Ok(result);
        }
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Category.Delete)]
        public async Task<IActionResult> RemoveCategory(Guid id)
        {
            if (id == null)
            {
                return BadRequest(new { message = "Id not null or empty" });
            }
            var result = await _category.DeleteCategory(id);
           
            _cacheManager.RemoveByPrefix("api/Category");
            return Ok(result);
        }
        [Cache]
        [HttpPost("categories-parent")]
        public async Task<IActionResult> GetCategoriesParent()
        {
            var result =await _category.GetCategoriesParent();
            return Ok(result);
        }


    }
}

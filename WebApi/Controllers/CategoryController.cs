using AutoMapper;
using Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Category;
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
        public async Task<IActionResult> GetCatogories(Guid id)
        {
            var result = await _category.GetCategory(id);
           
            // xóa cache

            return Ok(result);

        }
        [HttpPost("insert")]
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
    }
}

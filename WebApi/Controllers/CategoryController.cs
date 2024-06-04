using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Category;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        private readonly ICategoryService _category;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryService category, 
            IMapper mapper)
        {
            _category = category;
            _mapper = mapper;
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetCatogories()
        {
            var result = await _category.GetCategories();
            if (result.Errors == null || !result.Errors.Any())
            {
            }
            return Ok(result);

        }
        [HttpPost("{id}")]
        public async Task<IActionResult> GetCatogories(Guid id)
        {
            var result = await _category.GetCategory(id);
            if (result.Errors == null || !result.Errors.Any())
            {
            }
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
            if (result.Errors == null || !result.Errors.Any())
            {
            }
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
            if (result.Errors == null || !result.Errors.Any())
            {
            }
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
            if (result.Errors == null || !result.Errors.Any())
            {
            }
            return Ok(result);
        }
    }
}

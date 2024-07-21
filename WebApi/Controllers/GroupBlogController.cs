using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.BlogGroup.Request;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupBlogController : ControllerBase
    {
        private readonly IBlogGroupService _blogGroupService;
        public GroupBlogController(IBlogGroupService blogGroupService)
        {
            _blogGroupService = blogGroupService;
        }

        [HttpPost("create-blog-group")]
        public async Task<IActionResult> CretaeBlogGroup(BlogGroupRequestDto payload)
        {
            var result = await _blogGroupService.CreateBlogGroupAsync(payload);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetBlogGroup()
        {

            var result = await _blogGroupService.GetBlogGroupAsync();
            return Ok(result);
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateGroupBlog([FromBody] UpdateBlogGroupRequestDto payload)
        {
            var result = await _blogGroupService.UpdateBlogGroupAsync(payload);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveGroupBlog(Guid id)
        {
            bool check = await _blogGroupService.RemoveBlogGroupAsync(id);
            if(!check)
            {
                return BadRequest(new { message = $"Blog group remove is failed" });
            }
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogGroupDetail(Guid id)
        {
            var result = await _blogGroupService.GetBlogGroupDetailAsync(id);
            return Ok(result);
        }
    }
}

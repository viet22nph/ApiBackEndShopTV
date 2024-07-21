using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Blog.Request;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpPost("create-blog")]
        public async Task<IActionResult> CreateBlog(BlogRequestDto payload)
        {
            var result = await _blogService.CreateBlogAsync(payload);
            return Ok(result);
        }
        [HttpGet()]
        public async Task<IActionResult> GetBlogs(int pageNumber=  1, int pageSize = 20)
        {
            if(pageNumber < 1) {
                pageNumber = 1;
            }
            if(pageSize <1)
            {
                pageSize = 20;
            }
            var (data, count) = await _blogService.GetBlogAsync(pageNumber, pageSize);
            return Ok(new
            {
                message = data.Message,
                data = data.Data,
                pageNumber,
                pageSize,
                count
            });

        }
        [HttpDelete]
        public async Task<IActionResult> RemoveBlog(Guid id)
        {
            var checkRm = await _blogService.RemoveBlogAsync(id);
            if(!checkRm)
            {
                return BadRequest(new
                {
                    message = $"Not remove blog id = {id}"
                });
            }
            return NoContent();
        }
        [HttpPut("update-blog")]
        public async Task<IActionResult> UpdateBlog([FromBody] UpdateBlogRequestDto payload)
        {
            var data = await _blogService.UpdateBlogAsync(payload);
            return Ok(data);
        }
        [HttpGet("blog-group/{id}")]
        public async Task<IActionResult> GetBlogsByGroupId(Guid id ,int pageNumber = 1, int pageSize = 20)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            if (pageSize < 1)
            {
                pageSize = 20;
            }
            var (data, count) = await _blogService.GetBlogByGroupIdAsync(id,pageNumber, pageSize);
            return Ok(new
            {
                message = data.Message,
                data = data.Data,
                pageNumber,
                pageSize,
                count
            });

        }
        [HttpGet("blog-tag/{id}")]
        public async Task<IActionResult> GetBlogsByTagId(Guid id, int pageNumber = 1, int pageSize = 20)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            if (pageSize < 1)
            {
                pageSize = 20;
            }
            var (data, count) = await _blogService.GetBlogByTagIdAsync(id, pageNumber, pageSize);
            return Ok(new
            {
                message = data.Message,
                data = data.Data,
                pageNumber,
                pageSize,
                count
            });

        }

    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Tag.request;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpPost("create-tag")]
        public async Task<IActionResult> CreateTag(TagRequsetDto payload)
        {
            var result = await _tagService.CreateTagAsync(payload);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var result = await _tagService.GetTagAsync();
            return Ok(result);
        }


    }
}

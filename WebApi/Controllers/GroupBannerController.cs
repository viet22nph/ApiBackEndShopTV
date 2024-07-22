using Caching;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Settings;
using Services.Interfaces;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = Permissions.ManagerBanner.Manager)]
    public class GroupBannerController : ControllerBase
    {
        private readonly IGroupBannerService _groupBannerService;
        private readonly ICacheManager _cacheManager;

        public GroupBannerController(IGroupBannerService groupBannerService, ICacheManager cacheManager)
        {
            _groupBannerService = groupBannerService;
            _cacheManager = cacheManager;
        }
        [HttpGet()]
        [Cache()]
        public async Task<IActionResult> GetGroupBanner()
        {
            var result = await _groupBannerService.GetGroupBannerAsync();
            return Ok(result);
        }
        [HttpGet("groups-banner/{id}")]
        [Cache(5)]
        public async Task<IActionResult> GetDetailGroupBanner(Guid id)
        {
            if(id == Guid.Empty)
            {
                return BadRequest(new
                {
                    error = "param id is empty"
                });
            }
            
            var result = await _groupBannerService.GetDetailGroupBannerAsync(id);
            return Ok(result);
        }
        [HttpPut("toggle-enable/{id}")]
        public async Task<IActionResult> ToggleEnable(Guid id)
        {

            if (id == Guid.Empty)
            {
                return BadRequest(new
                {
                    error = "param id is empty"
                });
            }
            
            var result = await _groupBannerService.ToogleEnableAsync(id);
            _cacheManager.RemoveByPrefix("api/GroupBanner");
            return Ok(result);
        }
    }
}

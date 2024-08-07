﻿using Caching;
using Core.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Banner.Request;
using Models.Settings;
using Services.Interfaces;
using System.Net;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly IBannerService _bannerService;
        private readonly ICacheManager _cacheManager;

        public BannerController(IBannerService bannerService, ICacheManager cacheManager)
        {
            _bannerService = bannerService;
            _cacheManager = cacheManager;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.ManagerBanner.Manager)]
        [HttpPost("create-banner")]
        public async Task<IActionResult> CreateBanner(BannerRequestDto payload)
        {
            if (payload == null)
            {
                throw new ApiException($"Internal server error: Payload is null or empty")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            if (payload.GroupId == Guid.Empty)
            {
                return BadRequest(new
                {
                    error = $"Group id:  {payload.GroupId} is Guid empty"
                });
            }
            var result = await _bannerService.CreateBannerServiceAsync(payload);
            _cacheManager.RemoveByPrefix("api/Banner");
            _cacheManager.RemoveByPrefix("api/GroupBanner");
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.ManagerBanner.Manager)]
        [HttpPut("toggle-enable/{id}")]
        public async Task<IActionResult> ToggleEnable(Guid id)
        {
            if (id == Guid.Empty)
            {

                throw new ApiException($"Internal server error: Id is empty")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            var result = await _bannerService.ToogleEnableAsync(id);
            _cacheManager.RemoveByPrefix("api/Banner");
            _cacheManager.RemoveByPrefix("api/GroupBanner");
            return Ok(result);
        }
        [HttpGet()]
        [Cache()]
        public async Task<IActionResult> Banners(int pageNumber =1, int pageSize=20)
        {

            var result = await _bannerService.GetBannersAsync(pageNumber, pageNumber);
            return Ok(result);
        }
        [HttpDelete("{id}")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.ManagerBanner.Manager)]
        public async Task<IActionResult> RemoveBanner(Guid id)
        {
            var result = await _bannerService.RemoveBanner(id);
            if(!result)
            {
                new ApiException($"Internal server error: Remove banner id = {id} is failed")
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            _cacheManager.RemoveByPrefix("api/Banner");
            _cacheManager.RemoveByPrefix("api/GroupBanner");
            return NoContent();
        }
    }
}

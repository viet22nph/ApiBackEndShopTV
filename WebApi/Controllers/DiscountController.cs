using Caching;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Models.DTOs.Discount;
using Models.Settings;
using Services.Interfaces;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discount;
        private readonly ICacheManager _cacheManager;
        public DiscountController(IDiscountService discount, ICacheManager cacheManager)
        {
            _discount = discount;
            _cacheManager = cacheManager;
        }

        [HttpPost("create")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Discount.Create)]
        public async Task<IActionResult> CreateDiscount([FromBody] DiscountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
                var result = await _discount.CreateDiscount(request);
            _cacheManager.RemoveByPrefix("api/Product");
            _cacheManager.RemoveByPrefix("api/Discount");
            return Ok(result);
        }
        [HttpPost("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Discount.Read)]
        [Cache]
        public async Task<IActionResult> GetDiscount(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "Discount id is null or empty" });
            }
            var result = await _discount.GetDiscount(id);
            return Ok(result);
        }
        [HttpPost("list")]
        [Cache]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Discount.Read)]
        public async Task<IActionResult> GetDiscounts(int pageNumber =1, int pageSize = 10)
        {
            if(pageNumber < 1) {
                return BadRequest(new { message = "Page number must be greater than or equal to 1" });
            }
            if(pageSize < 1)
            {
                return BadRequest(new { message = "Page Size must be greater than or equal to 1" });
            }
            var result = await _discount.GetDiscounts(pageNumber, pageSize);

            return Ok(result);
        }

        [HttpPost("cancel")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Discount.Update)]
        public async Task<IActionResult> CancelDiscount([FromBody] DiscountIdDto payload)
        {
            if (payload.DiscountId == null ||  payload.DiscountId == Guid.Empty)
            {
                return BadRequest(new { message = "Id discount not null or empty" });
            }
            var result = await _discount.CancelledDiscountStatus(payload.DiscountId.Value);
            _cacheManager.RemoveByPrefix("api/Product");
            _cacheManager.RemoveByPrefix("api/Discount");
            return Ok(result);
        }
        [HttpPost("pause")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Discount.Update)]
        public async Task<IActionResult> PauseDiscount([FromBody] DiscountIdDto payload)
        {
            if (payload.DiscountId == null || payload.DiscountId == Guid.Empty)
            {
                return BadRequest(new { message = "Id discount not null or empty" });
            }
            var result = await _discount.PauseDiscountStatus(payload.DiscountId.Value);

            _cacheManager.RemoveByPrefix("api/Product");
            _cacheManager.RemoveByPrefix("api/Discount");
            return Ok(result);
        }
        [HttpPost("continue")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Discount.Update)]
        public async Task<IActionResult> ContinueDiscount([FromBody] DiscountIdDto payload)
        {
            if (payload.DiscountId == null || payload.DiscountId == Guid.Empty)
            {
                return BadRequest(new { message = "Id discount not null or empty" });
            }
            var result = await _discount.ContinueDiscountStatus(payload.DiscountId.Value);

            _cacheManager.RemoveByPrefix("api/Product");
            _cacheManager.RemoveByPrefix("api/Discount");
            return Ok(result);
        }
        [HttpPatch("update-time/{id}")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Discount.Update)]
        public async Task<IActionResult> UpdateDatetime(Guid id, [FromBody] DiscountDateTimeRequest request)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "Id discount not null or empty" });
            }

            var result = await _discount.UpdateDateTime(id, request);

            _cacheManager.RemoveByPrefix("api/Product");
            _cacheManager.RemoveByPrefix("api/Discount");
            return Ok(result);
        }
    }
}

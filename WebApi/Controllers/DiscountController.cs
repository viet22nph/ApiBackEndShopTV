using Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Models.DTOs.Discount;
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
        public async Task<IActionResult> CreateDiscount([FromBody] DiscountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
                var result = await _discount.CreateDiscount(request);
            _cacheManager.RemoveByPrefix("api/Discount");
            return Ok(result);
        }
        [HttpPost("{id}")]
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
        public async Task<IActionResult> CancelDiscount([FromBody] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "Id discount not null or empty" });
            }
            var result = await _discount.CancelledDiscountStatus(id);
            _cacheManager.RemoveByPrefix("api/Discount");
            return Ok(result);
        }
        [HttpPost("pause")]
        public async Task<IActionResult> PauseDiscount([FromBody] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "Id discount not null or empty" });
            }
            var result = await _discount.PauseDiscountStatus(id);
            _cacheManager.RemoveByPrefix("api/Discount");
            return Ok(result);
        }
        [HttpPost("continue")]
        public async Task<IActionResult> ContinueDiscount([FromBody] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "Id discount not null or empty" });
            }
            var result = await _discount.ContinueDiscountStatus(id);
            _cacheManager.RemoveByPrefix("api/Discount");
            return Ok(result);
        }
        [HttpPatch("update-time/{id}")]
        public async Task<IActionResult> UpdateDatetime(Guid id, [FromBody] DiscountDateTimeRequest request)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "Id discount not null or empty" });
            }

            var result = await _discount.UpdateDateTime(id, request);
            _cacheManager.RemoveByPrefix("api/Discount");
            return Ok(result);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Review;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }


        [HttpPost("create-review")]
        public async Task<IActionResult> CreateReview([FromBody] ReviewRequest review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _reviewService.CreateReview(review);

            return Ok(result);
        }
        [HttpPost("{productId}")]
        public async Task<ActionResult> GetReviews(Guid productId, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var (reviews, totalCount, averageRating) = await _reviewService.GetReviewsByProductId(productId, pageNumber, pageSize);

            var result = new
            {
                TotalCount = totalCount,
                AverageRating = averageRating,
                PageNumber = pageNumber,
                PageSize = pageSize,
                message = reviews.Message,
                data = reviews.Data
            };

            return Ok(result);
        }
        [HttpDelete("/{id}")]
        public async Task<IActionResult> RemoveReviews(Guid id)
        {
            await _reviewService.RemoveReview(id);
            return NoContent();
        }
    }
}

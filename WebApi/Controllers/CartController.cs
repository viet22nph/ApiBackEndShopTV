using Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.DTOs.Cart;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] CartRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserId) || request.ProductItemId == Guid.Empty|| request.Quantity <= 0)
            {
                return BadRequest("Invalid cart request.");
            }
            await _cartService.AddToCart(request);
            return NoContent();
        }

        [HttpPost("get-cart")]
        public async Task<IActionResult> GetCartUser([FromBody] string id)
        {
            var cart = await _cartService.GetCart(id);
            return Ok(cart);
        }
        [HttpPost("remove-cart")]
        public async Task<IActionResult> RemoveCart([FromBody] CartRemoveRequest request)
        {
            var result = await _cartService.DeleteFromCart(request.UserId, request.ProductItemId);
            return Ok(result);
        }
    }
}

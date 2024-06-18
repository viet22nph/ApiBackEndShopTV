using Caching;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.DTOs.Cart;
using Services.Interfaces;
using System.Data;

namespace WebApi.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            if (request == null || string.IsNullOrEmpty(request.UserId) || request.ProductItemId == Guid.Empty)
            {
                return BadRequest("Invalid cart request.");
            }
            await _cartService.AddToCart(request);
            return NoContent();
        }

        [HttpPost("get-cart/{id}")]
        public async Task<IActionResult> GetCartUser(string id)
        {
            var cart = await _cartService.GetCart(id);
            return Ok(cart);
        }

        [HttpDelete("{userId}/item/{itemId}")]
        public async Task<IActionResult> RemoveCart(string userId, Guid itemId)
        {
            var result = await _cartService.DeleteFromCart(userId, itemId);
            return Ok(result);
        }
    }
}

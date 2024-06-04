using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Order;
using Org.BouncyCastle.Crypto.Engines;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("review-checkout")]
        public async Task<IActionResult> CheckoutReview([FromBody] ReviewCheckoutRequest request)
        {
            
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }    

            var result =  await _orderService.ReviewCheckoutOrder(request);
            return Ok(result);
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _orderService.CreateOrder(request);
            return Ok(result);
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetOrders(int pageNumber = 1, int pageSize = 10)
        {

            var res = await _orderService.GetListOrder(pageNumber, pageSize);
            return Ok(res);
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {

            var res = await _orderService.GetOrderDetail(id);
            return Ok(res);
        }
        [HttpPost("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] OrderUpdateStatusRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }    

            var result = await _orderService.UpdateStatus(request);
            return Ok(result);
        }


    }
}

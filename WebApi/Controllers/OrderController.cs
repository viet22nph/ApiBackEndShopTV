using Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Models.DTOs.Order;
using Org.BouncyCastle.Crypto.Engines;
using Services.Interfaces;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICacheManager _cacheManager;
        private readonly ICartService _cartService;
        public OrderController(IOrderService orderService, ICacheManager cacheManager, ICartService cartService)
        {
            _cacheManager = cacheManager;
            _orderService = orderService;
            _cartService = cartService;
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

            if(result.Data.UserId != null)
            {
                await _orderService.SendMailOrder(result.Data.Id);
                // clear cart

                foreach (var item in request.Items)
                {
                    await _cartService.DeleteFromCart(result.Data.UserId, item.ProductItemId);
                }
            }    
           
            _cacheManager.RemoveByPrefix("api/Product");
            _cacheManager.RemoveByPrefix("api/Order");
            return Ok(result);
        }

        [HttpPost("list")]
        [Cache]
        public async Task<IActionResult> GetOrders(int pageNumber = 1, int pageSize = 10)
        {

            var (result, count) = await _orderService.GetListOrder(pageNumber, pageSize);
            return Ok(new
            {
                message =result.Message,
                data = result.Data, 
                pageNumber = pageNumber,
                pageSize = pageSize,
                total = count

            });
        }
        [HttpPost("{id}")]
        [Cache(300)]
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
            _cacheManager.RemoveByPrefix("api/Product");
            _cacheManager.RemoveByPrefix("api/Order");
            return Ok(result);
        }

        [HttpPost("user/{id}")]
        public async Task<IActionResult> GetOrdersByUserId(string id)
            {
            var result = await _orderService.GetOrdersByUserId(id);
            return Ok(result);
        }

        [HttpPost("total-revenue-last-month")]
        public async Task<IActionResult> GetTotalRevenueLastMonth()
        {
            var (total, dateStart, dateEnd) = await _orderService.GetTotalRevenueLastMonth();
            var result = new
            {
                data = new
                {
                    total = total,
                    dateStart = dateStart,
                    dateEnd = dateEnd,
                },
                message = "total revenue last month"
            };
            return Ok(result);
        }
        [HttpPost("order-by-date")]
        public async Task<IActionResult> GetOrderByDate(DateTime date, int pageSize, int pageNumber)
        {
            var (result, count) = await _orderService.GetListOrderByDate(date, pageSize,pageNumber);
            return Ok(new
            {
                messaege = $"Order date {date.Date}",
                data = result,
                pageSize,
                pageNumber,
                total = count
            }) ;
            
        }
    }
}

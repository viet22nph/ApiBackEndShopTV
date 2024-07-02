using Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Order;
using Models.RequestModels;
using Models.Status;
using Newtonsoft.Json;
using Services.Concrete;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        private readonly IVnPayService _vnPay;
        private readonly IOrderService _order;
        private readonly ICartService _cart;
        private readonly ICacheManager _cacheManager;
        public PaymentController(IVnPayService vnPay, IOrderService order, ICartService cart, ICacheManager cacheManager)
        {
            _vnPay = vnPay;
            _order = order;
            _cart = cart;
            _cacheManager = cacheManager;
        }
        [HttpPost("vnpay")]
        public async Task<IActionResult> PaymentVnpay([FromBody] OrderRequest request)
        {

            Guid id = Guid.NewGuid();
            await _cacheManager.SetAsync($"OrderPayment:{id}", request, 15);
           
            var vnpayRequest = new VnpayRequest()
            {
                OrderId = id,
                UserId = request.UserId,
                Amount = request.Total
            };

            return Ok(new {
                Url = _vnPay.CreateVnpayUrl(HttpContext, vnpayRequest)
            });
        }
        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn()
        {
            var res = _vnPay.PaymentExecute(Request.Query);

            if (res.Success)
            {

                if (res.VnPayResponseCode == "00")
                {
                    var dataCache = await _cacheManager.GetAsync($"OrderPayment:{res.OrderId}");
                    var order = JsonConvert.DeserializeObject<OrderRequest>(dataCache);
                    order.Status = OrderStatus.COMPLETED;
                    order.Transactions.First().Status = TransactionStatus.COMPLETED;
                    var result = await _order.CreateOrder(order);
                    if (result.Data.UserId != null)
                    {
                        await _order.SendMailOrder(result.Data.Id);
                        // clear cart

                        foreach (var item in order.Items)
                        {
                            await _cart.DeleteFromCart(result.Data.UserId, item.ProductItemId);
                        }
                    }
                    _cacheManager.RemoveByPrefix("api/Product");
                    _cacheManager.RemoveByPrefix("api/Order");
                    return Ok(new { RspCode = "00", Message = "Confirm Success" });
                }
                else
                {
                    return Ok(new { RspCode = "01", Message = "Confirm Fail" });
                }
            }
            else
            {
                await _order.RemoveOrder(res.OrderId);
                return BadRequest(new { RspCode = "97", Message = "Invalid signature" });
            }
        }

    }
}

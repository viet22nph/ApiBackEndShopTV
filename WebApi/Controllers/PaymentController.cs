using Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Order;
using Models.RequestModels;
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
            var orderResponse = await _order.CreateOrder(request);
            if (orderResponse == null)
            {
                return BadRequest(new
                {
                    message = "Payment Error"
                });
            }
            var vnpayRequest = new VnpayRequest()
            {
                OrderId = orderResponse.Data.Id,
                UserId = orderResponse?.Data.UserId,
                Amount = orderResponse.Data.GrandTotal
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
                    await _order.UpdateStatus(new OrderUpdateStatusRequest
                    {
                        Id = res.OrderId,
                        Status = "COMPLETED"
                    });
                    var order = await _order.GetOrderDetail(res.OrderId);
                    if (res.OrderId != null)
                    {
                        await _order.SendMailOrder(order.Data.OrderId);
                        // clear cart

                        foreach (var item in order.Data.OrderItems)
                        {
                            await _cart.DeleteFromCart(order.Data.UserId, item.ProductItemId);
                        }
                    }

                    _cacheManager.RemoveByPrefix("api/Product");
                    _cacheManager.RemoveByPrefix("api/Order");
                    return Ok(new { RspCode = "00", Message = "Confirm Success" });
                }
                else
                {
                    await _order.RemoveOrder(res.OrderId);
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

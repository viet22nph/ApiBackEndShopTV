using Application.DAL.Helper;
using Application.DAL.Models;
using AutoMapper;
using Caching;
using Core.Exceptions;
using Core.Interfaces;
using Data.Contexts;
using Data.Repos.DiscountRepo;
using Data.Repos.OrderRepo;
using Data.Repos.ProductRepo;
using Data.UnitOfWork;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models.DTOs.Email;
using Models.DTOs.Order;
using Models.ResponseModels;
using Models.Settings;
using Models.Status;
using Services.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly IEmailCoreService _emailService;
        private readonly ICacheManager _cacheManager;
        private readonly MailSettings _mailSetting;
        public OrderService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ApplicationDbContext context,
            IEmailCoreService emailService,
            ICacheManager cacheManager,
            IOptions<MailSettings> mailSetting
            )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _emailService = emailService;

            _context = context;
            _cacheManager = cacheManager;
            _mailSetting = mailSetting.Value;
        }

        public async Task<BaseResponse<OrderDto>> CreateOrder(OrderRequest request)
        {
            bool isSuccess = true;
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Application.DAL.Models.Order
                {
                    OrderType = request.OrderType,
                    Address = request.Address,
                    Phone = request.Phone,
                    RecipientName = request.RecipientName,
                    SubTotal = request.SubTotal,
                    Total = request.Total,
                    DateCreate = DateTime.Now,
                    TotalDiscount = request.TotalDiscount,

                    Notes = request.Notes,
                    UserId = request.UserId,
                    Status = request.Status ?? OrderStatus.NEWORDER, // Assuming a new order status
                    OrderItems = request.Items.Select(s =>
                    {
                        return new OrderItem
                        {
                            ProductItemId = s.ProductItemId,
                            Quantity = s.Quantity,
                            Price = s.Price,
                            AmountDiscount = s.AmountDiscount
                        };
                    }).ToList(),
                    Transactions = request.Transactions.Select(t =>
                    {
                        return new Transaction
                        {
                            Amount = t.Amount,
                            DateCreate = DateTime.Now,
                            Description = t.Description,
                            Status = t.Status ?? TransactionStatus.PENDING,
                            UserId = t.UserId,
                            Type = t.Type
                        };
                    }).ToList(),

                };
                // Tru san pham 
                foreach (var orderItem in request.Items)
                {
                    var product = await _unitOfWork.ProductRepository.GetProductItem(orderItem.ProductItemId);
                    if (product == null)
                    {
                        throw new ApiException($"Internal server error: Product item is null or empty")
                        { StatusCode = (int)HttpStatusCode.BadRequest };
                    }
                    product.Quantity -= orderItem.Quantity;
                    product.Product.ProductQuantity -= orderItem.Quantity;
                    if (product.Product.ProductQuantity < 0 || product.Product.ProductQuantity < 0)
                    {

                        isSuccess = false;
                        order.Status = OrderStatus.FAILED;

                        product.Quantity += orderItem.Quantity;
                        product.Product.ProductQuantity += orderItem.Quantity;

                    }
                    await _unitOfWork.Repository<ProductItem>().Update(product);
                }

                order = await _unitOfWork.Repository<Application.DAL.Models.Order>().Insert(order);
                if (order == null)
                {
                    throw new ApiException($"Internal server error: Insert order failed")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }

                var res = _mapper.Map<OrderDto>(order);
                await transaction.CommitAsync();
                return new BaseResponse<OrderDto>(res, "Order");

            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync();
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }


        }


        public async Task<(BaseResponse<ICollection<OrderDto>>, int)> GetListOrder(int pageNumber, int pageSize)
        {
            try
            {
                var (orders, count) = await _unitOfWork.OrderRepository.GetOrders(pageNumber, pageSize);
                if (orders == null)
                {
                    throw new ApiException($"Not found")
                    { StatusCode = (int)HttpStatusCode.NotFound };
                }
                var res = _mapper.Map<List<OrderDto>>(orders);
                return (new BaseResponse<ICollection<OrderDto>>(res, "Orders"), count);

            }
            catch (Exception ex)
            {

                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }

        public async Task<BaseResponse<OrderDetailDto>> GetOrderDetail(Guid id)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.GetOrderDetail(id);
                if (order == null)
                {
                    throw new ApiException($"Not found")
                    { StatusCode = (int)HttpStatusCode.NotFound };
                }
                var rs = mapOrderDetail(order);

                return new BaseResponse<OrderDetailDto>(rs, "Order");
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }


        }

        public async Task<BaseResponse<ICollection<OrderDto>>> GetOrdersByUserId(string userId)
        {
            try
            {
                var data = await _unitOfWork.OrderRepository.GetOrderByUserId(userId);
                if (data == null)
                {
                    throw new ApiException($"Not found")
                    { StatusCode = (int)HttpStatusCode.NotFound };
                }
                var res = _mapper.Map<List<OrderDto>>(data);
                return new BaseResponse<ICollection<OrderDto>>(res, "Orders");

            }
            catch (Exception ex)
            {

                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }
        public async Task<BaseResponse<ReviewCheckoutResponse>> ReviewCheckoutOrder(ReviewCheckoutRequest request)
        {
            try
            {    // duyệt items
                var reviewCheckoutRes = new ReviewCheckoutResponse();
                foreach (var item in request.Items)
                {
                    var productItem = await _unitOfWork.ProductRepository.GetProductItem(item.ProductItemId);
                    var reviewCheckoutReviewItem = new ReviewCheckoutItem();

                    if (productItem == null)
                    {
                        throw new ApiException($"Product item not found")
                        { StatusCode = (int)HttpStatusCode.NotFound };
                    }
                    decimal discountAmount = 0;
                    if (productItem.Product.Discount != null)
                    {
                        var discount = productItem.Product.Discount;
                        if (discount.Status == DiscountStatus.ACTIVE && discount.DateStart <= DateTime.Now && discount.DateEnd >= DateTime.Now)
                        {

                            if (item.Quantity * productItem.Product.Price >= discount.MinimumPurchase)
                            {

                                if (discount.Type == DiscountType.PERCENTAGE)
                                {
                                    discountAmount = (productItem.Product.Price * item.Quantity * discount.DiscountValue) / 100;
                                }
                                else if (discount.Type == "fix-amount")
                                {
                                    discountAmount = discount.DiscountValue;
                                }

                            }
                        }
                    }
                    reviewCheckoutReviewItem.ProductItemId = productItem.Id;
                    reviewCheckoutReviewItem.ProductName = productItem.Product.Name;
                    reviewCheckoutReviewItem.Quantity = item.Quantity;
                    reviewCheckoutReviewItem.Image = productItem?.ProductImages.First().Url;
                    reviewCheckoutReviewItem.Price = productItem.Product.Price;
                    reviewCheckoutReviewItem.AmountDiscount = discountAmount;
                    reviewCheckoutRes.ReviewCheckoutItems.Add(reviewCheckoutReviewItem);
                }
                reviewCheckoutRes.SubTotal = reviewCheckoutRes.ReviewCheckoutItems.Sum(s => s.Total);
                reviewCheckoutRes.DiscountAmount = reviewCheckoutRes.ReviewCheckoutItems.Sum(s => s.AmountDiscount);
                reviewCheckoutRes.Total = reviewCheckoutRes.SubTotal - reviewCheckoutRes.DiscountAmount;
                return new BaseResponse<ReviewCheckoutResponse>(reviewCheckoutRes, "checkout");

            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }

        public async Task<BaseResponse<string>> UpdateStatus(OrderUpdateStatusRequest request)
        {
            if (!OrderStatus.IsValidStatus(request.Status))
            {
                throw new ApiException($"Invalid status value")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _unitOfWork.OrderRepository.GetOrderDetail(request.Id);
                if (order == null)
                {
                    throw new ApiException($"Not found")
                    { StatusCode = (int)HttpStatusCode.NotFound };
                }
                if (order.Status == OrderStatus.COMPLETED || order.Status == OrderStatus.CANCELLED)
                {

                    throw new ApiException($"Cannot update status of a completed or cancelled order")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                if (request.Status == OrderStatus.CANCELLED)
                {
                    foreach (var orderItem in order.OrderItems)
                    {
                        var product = orderItem.Product;
                        product.Quantity += orderItem.Quantity;
                        product.Product.ProductQuantity += orderItem.Quantity;
                        await _unitOfWork.Repository<ProductItem>().Update(product);
                    }


                }

                order.Status = request.Status;
                order.DateUpdate = DateTime.UtcNow;
                order = await _unitOfWork.Repository<Application.DAL.Models.Order>().Update(order);
                if (order == null)
                {

                    await _context.Database.RollbackTransactionAsync();
                    throw new ApiException($"Cannot update status")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                order.DateUpdate = DateTime.Now;
                await _context.Database.CommitTransactionAsync();
                return new BaseResponse<string>("Order status updated successfully");
            }
            catch (Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }
        public async Task<(decimal totalRevenue, DateTime startDate, DateTime endDate)> GetTotalRevenueLastMonth()
        {
            var lastMonth = DateTime.Now.AddMonths(-1);
            var startOfLastMonth = new DateTime(lastMonth.Year, lastMonth.Month, 1);
            var endOfLastMonth = startOfLastMonth.AddMonths(1).AddDays(-1);

            var totalRevenue = await _unitOfWork.OrderRepository.GetTotalRevenue(new DateTime(2024, 6, 1), new DateTime(2024, 6, 30));
            return (totalRevenue, startOfLastMonth, endOfLastMonth);
        }
        public async Task SendMailOrder(Guid orderId)
        {

            var order = await _unitOfWork.OrderRepository.GetOrderDetail(orderId);
            if (order.UserId != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == order.UserId);
                await _emailService.SendAsync(new EmailRequest
                {
                    To = user?.Email, // Set the recipient email address here
                    From = _mailSetting.SmtpUser, // Set the sender email address here
                    Subject = "ShopTV. Hóa đơn mới cho đơn hàng của quý khách đã được tạo",
                    Body = GenerateHtmlBody(order), // Specify that the email body is HTML
                });
            }
        }

        public async Task<bool> CheckOrderBeforeCreate(OrderRequest request)
        {


            foreach (var orderItem in request.Items)
            {
                var product = await _unitOfWork.ProductRepository.GetProductItem(orderItem.ProductItemId);
                if (product == null)
                {
                    throw new ApiException($"Internal server error: Product item is null or empty")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                product.Quantity -= orderItem.Quantity;
                product.Product.ProductQuantity -= orderItem.Quantity;
                if (product.Product.ProductQuantity < 0 || product.Product.ProductQuantity < 0)
                {
                    throw new ApiException($"Internal server error: Product quantity is not enough ")
                    { StatusCode = (int)HttpStatusCode.BadRequest };

                }
            }


            return true;
        }
        public async Task<bool> RemoveOrder(Guid orderId)
        {
            try
            {
                var rs = await _unitOfWork.OrderRepository.RemoveOrder(orderId);
                if (rs == false)
                {
                    throw new ApiException($"Internal server error: Not found order")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                return rs;
            }
            catch (Exception ex)
            {

                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };

            }


        }
        public async Task<(ICollection<OrderDetailDto>, int)> GetListOrderByDate(DateTime date, int pageSize, int pageNumber)
        {
            var (orders, count) = await _unitOfWork.OrderRepository.GetListOrderByDate(date, pageSize, pageNumber);
            if (orders == null)
            {
                return ([], count);
            }
            var data = orders.Select(x =>
                {
                    return mapOrderDetail(x);
                }).ToList();
            return (data, count);
        }
        private OrderDetailDto mapOrderDetail(Application.DAL.Models.Order order)
        {

            var rs = new OrderDetailDto
            {
                OrderId = order.Id,
                UserId = order.UserId,
                OrderType = order.OrderType,
                Address = order.Address,
                Phone = order.Phone,
                RecipientName = order.RecipientName,
                SubTotal = order.SubTotal,
                Total = order.Total,
                TotalDiscount = order.TotalDiscount,
                Status = order.Status,
                Notes = order.Notes,
                DateCreate = order.DateCreate,
                DateUpdate = order.DateUpdate,
            };
            rs.OrderItems = order.OrderItems == null ? null : order.OrderItems?.Select(oi => new OrderDetailDto.OrderItem()
            {
                OrderId = oi.OrderId,
                ProductItemId = oi.ProductItemId,
                Quantity = oi.Quantity,
                Price = oi.Price,
                Product = oi.Product != null ? new OrderDetailDto.OrderItem.ProductItem()
                {
                    ProductId = oi.Product.ProductId,

                    ProductName = oi?.Product?.Product?.Name ?? null,
                    Image = oi?.Product?.ProductImages?.Count > 0 ? oi?.Product.ProductImages.First().Url : null,
                    ColorItem = oi?.Product?.Color != null ? new OrderDetailDto.OrderItem.ProductItem.Color
                    {
                        Id = oi.Product.Color.Id,
                        ColorName = oi.Product.Color.ColorName,
                        ColorCode = oi.Product.Color.ColorCode
                    } : null,

                } : null
            }).ToList();
            rs.Transactions = order.Transactions == null ? null : order.Transactions?.Select(t =>
            {
                return new OrderDetailDto.Transaction
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    Amount = t.Amount,
                    Type = t.Type,
                    Description = t.Description,
                    Status = t.Status,
                    DateCreate = t.DateCreate,
                    DateUpdate = t.DateUpdate
                };
            }).ToList();
            return rs;
        }
        private string GenerateHtmlBody(Application.DAL.Models.Order order)
        {
            // Tạo phần đầu của HTML
            var html = new StringBuilder();
            html.Append(@$"<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Phản hồi yêu cầu của bạn</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
        }}
        .email-container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}
        .email-header {{
            background-color: #005c29;
            color: #ffffff;
            padding: 10px;
            text-align: center;
            border-top-left-radius: 8px;
            border-top-right-radius: 8px;
        }}
        .email-body {{
            padding: 20px;
            color: #333333;
        }}
        .email-footer {{
            text-align: center;
            padding: 10px;
            background-color: #005c29;
            color: #ffffff;
            border-bottom-left-radius: 8px;
            border-bottom-right-radius: 8px;
        }}
        .button {{
            display: inline-block;
            padding: 10px 20px;
            margin: 10px 0;
            color: #ffffff;
            background-color: #005c29;
            text-decoration: none;
            border-radius: 5px;
        }}
        .button:hover {{
            background-color: #002244;
        }}
        table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }}
        th, td {{
            border: 1px solid #dddddd;
            padding: 8px;
            text-align: left;
        }}
        th {{
            background-color: #f4f4f4;
        }}
        .total {{
            font-weight: bold;
        }}
        .product-image {{
            width: 100px; /* Adjust size as needed */
            height: auto;
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""email-header"">
            <h1>TVfurni Shop</h1>
        </div>
        <div class=""email-body"">
            <p>Xin chào Anh (Chị) {order.RecipientName},</p>
            <p>Cảm ơn Anh (Chị) đã ủng hộ của hàng! Dưới đây là thông tin chi tiết về đơn hàng của Anh (Chị):</p>
            <h2>Thông Tin Hóa Đơn</h2>
            <table>
                <tr>
                    <th>Số Hóa Đơn</th>
                    <td>{order.Id}</td>
                </tr>
                <tr>
                    <th>Ngày Hóa Đơn</th>
                    <td>{order.DateCreate.Date:dd/MM/yyyy}</td>
                </tr>
                <tr>
                    <th>Địa Chỉ Giao Hàng</th>
                    <td>{order.Address}</td>
                </tr>
            </table>

            <h2>Chi Tiết Đơn Hàng</h2>
            <table>
                <thead>
                    <tr>
                        <th>Hình Ảnh Sản Phẩm</th>
                        <th>Tên Sản Phẩm</th>
                        <th>Số Lượng</th>
                        <th>Giá</th>
                        <th>Tổng Cộng</th>
                    </tr>
                </thead>
                <tbody>");

            // Duyệt qua danh sách sản phẩm trong đơn hàng
            foreach (var item in order.OrderItems)
            {
                html.Append($@"
                    <tr>
                        <td><img src=""{item?.Product?.ProductImages?.First().Url}"" alt=""item"" class=""product-image""></td>
                        <td>{item?.Product?.Product?.Name}</td>
                        <td>{item?.Quantity}</td>
                        <td>₫{item?.Price:N0}</td>
                        <td>₫{(item?.Price * item?.Quantity):N0}</td>
                    </tr>");
            }


            html.Append($@"
                </tbody>
                <tfoot>
                     <tr>
                        <td colspan=""4"" class=""total"">Tổng Cộng</td>
                        <td class=""total"">₫{order.SubTotal:N0}</td>
                    </tr>
                    <tr>
                        <td colspan=""4"" class=""total"">Tổng Cộng</td>
                        <td class=""total"">₫{order.TotalDiscount:N0}</td>
                    </tr>
                    <tr>
                        <td colspan=""4"" class=""total"">Thành tiền</td>
                        <td class=""total"">₫{order.Total:N0}</td>
                    </tr>
                </tfoot>
            </table>

            <p>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi:</p>
            <p>Email: support@furnishop.com<br>Điện thoại: (123) 456-7890</p>
            <p>Trân trọng,</p>
            <p>Đội ngũ Furni Shop</p>
            <a href=""https://furnishop.com"" class=""button"">Truy Cập Trang Web Của Chúng Tôi</a>
        </div>
        <div class=""email-footer"">
            <p>&copy; 2024 Furni Shop. Bảo lưu tất cả các quyền.</p>
        </div>
    </div>
</body>
</html>");

            return html.ToString();
        }
    }
}

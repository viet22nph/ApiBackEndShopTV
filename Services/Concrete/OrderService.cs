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
using Microsoft.IdentityModel.Tokens;
using Models.DTOs.Email;
using Models.DTOs.Order;
using Models.ResponseModels;
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
        public OrderService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ApplicationDbContext context,
            IEmailCoreService emailService,
            ICacheManager cacheManager
            )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _emailService = emailService;

            _context = context;
            _cacheManager = cacheManager;
        }

        public async Task<BaseResponse<OrderDto>> CreateOrder(OrderRequest request)
        {
            await _context.Database.BeginTransactionAsync();
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
                            Status = t.Status?? TransactionStatus.PENDING,
                            UserId = t.UserId,
                            Type = t.Type
                        };
                    }).ToList(),
                 
                };
                // Tru san pham 
                foreach(var orderItem in request.Items)
                {
                    var product = await _unitOfWork.ProductRepository.GetProductItem(orderItem.ProductItemId);
                    if(product == null)
                    {
                        await _context.Database.RollbackTransactionAsync();
                        throw new ApiException($"Internal server error: Product item is null or empty")
                        { StatusCode = (int)HttpStatusCode.BadRequest };
                    }    
                    product.Quantity -= orderItem.Quantity;
                    product.Product.ProductQuantity -= orderItem.Quantity;
                    if(product.Product.ProductQuantity < 0 || product.Product.ProductQuantity< 0) {
                        await _context.Database.RollbackTransactionAsync();
                        throw new ApiException($"Internal server error: Not enough product quantity")
                        { StatusCode = (int)HttpStatusCode.BadRequest };
                    }
                    await _unitOfWork.Repository<ProductItem>().Update(product);
                }
                
                order = await _unitOfWork.Repository<Application.DAL.Models.Order>().Insert(order);
                if(order == null)
                {
                    await _context.Database.RollbackTransactionAsync();
                    throw new ApiException($"Internal server error: Insert order failed")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                
                
                // xoa gio hang
                
                var res = _mapper.Map<OrderDto>(order);
                await _context.Database.CommitTransactionAsync();
                return new BaseResponse<OrderDto>(res, "Order");

            }
            catch(Exception ex)
            {

                await _context.Database.RollbackTransactionAsync();
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }


        }


        public  async Task<BaseResponse<ICollection<OrderDto>>> GetListOrder(int pageNumber, int pageSize)
        {
            try
            {
                var data = await _unitOfWork.OrderRepository.GetOrders(pageNumber, pageSize);
                if(data == null)
                {
                    throw new ApiException($"Not found")
                    { StatusCode = (int)HttpStatusCode.NotFound };
                }    
                var res = _mapper.Map<List<OrderDto>>(data);
                return new BaseResponse<ICollection<OrderDto>>(res, "Orders");
                
            }catch(Exception ex)
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

                    if(productItem == null) {
                        throw new ApiException($"Product item not found")
                        { StatusCode = (int)HttpStatusCode.NotFound };
                    }
                    decimal discountAmount = 0;
                    if (productItem.Product.Discount!= null)
                    {
                        var discount = productItem.Product.Discount;
                        if(discount.Status == DiscountStatus.ACTIVE && discount.DateStart <= DateTime.Now && discount.DateEnd >= DateTime.Now)
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
                reviewCheckoutRes.GrandTotal = reviewCheckoutRes.SubTotal - reviewCheckoutRes.DiscountAmount;
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
            bool isCancelled = false;
            if (!OrderStatus.IsValidStatus(request.Status))
            {
                throw new ApiException($"Invalid status value")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _unitOfWork.OrderRepository.GetOrderDetail(request.Id);
                if(order == null)
                {
                    throw new ApiException($"Not found")
                    { StatusCode = (int)HttpStatusCode.NotFound };
                }
                if(order.Status == OrderStatus.COMPLETED || order.Status == OrderStatus.CANCELLED)
                {

                    throw new ApiException($"Cannot update status of a completed or cancelled order")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                if(request.Status == OrderStatus.CANCELLED)
                {
                    // update lai san pham
                    isCancelled = true;
                    foreach(var orderItem in order.OrderItems)
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
                if(order == null )
                {

                    await _context.Database.RollbackTransactionAsync();
                    throw new ApiException($"Cannot update status")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                await _context.Database.CommitTransactionAsync();
                return new BaseResponse<string>("Order status updated successfully");
            }
            catch(Exception ex)
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
                    To = user.Email, // Set the recipient email address here
                    From = "nguyendinh.viet2002np@gmail.com", // Set the sender email address here
                    Subject = "ShopTV. Hóa đơn mới cho đơn hàng của quý khách đã được tạo",
                    Body = GenerateHtmlBody(order), // Specify that the email body is HTML
                });
            }
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
            }catch (Exception ex)
            {
              
                    throw new ApiException($"Internal server error: {ex.Message}")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                
            }
         

        }
        public async Task<ICollection<OrderDetailDto>> GetListOrderByDate(DateTime date)
        {
            var orders = await _unitOfWork.OrderRepository.GetListOrderByDate(date);
            if (orders == null)
            {
                return [];
            }
            var data = orders.Select(x =>
                {
                    return mapOrderDetail(x);
                }).ToList();
            return data;
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
                    Image = oi?.Product.ProductImages == null ? oi?.Product.ProductImages.First().Url : null,
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
            var html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("<body>");
            html.AppendLine("<h2>Đơn hàng mới</h2>");
            html.AppendLine("<p>Cảm ơn bạn đã đặt hàng. Dưới đây là thông tin đơn hàng của bạn.</p>");

            html.AppendLine("<h3>Thông tin đơn hàng</h3>");
            html.AppendLine("<ul>");
            html.AppendLine($"<li><strong>Mã hóa đơn:</strong> {order.Id}</li>");
            html.AppendLine($"<li><strong>Loại hóa đơn:</strong> {order.OrderType}</li>");
            html.AppendLine($"<li><strong>Địa chỉ:</strong> {order.Address}</li>");
            html.AppendLine($"<li><strong>Số điện thoại:</strong> {order.Phone}</li>");
            html.AppendLine($"<li><strong>Tên người nhận:</strong> {order.RecipientName}</li>");
            html.AppendLine($"<li><strong>Tổng phụ:</strong> {order.SubTotal.ToString("#,##0")} VNĐ</li>");
            html.AppendLine($"<li><strong>Tổng cộng:</strong> {order.Total.ToString("#,##0")} VNĐ</li>");
            html.AppendLine("</ul>");
            html.AppendLine("<p>Phương thức vận chuyển và phí vận chuyển của đơn hàng sẽ được cập nhật sau khi nhân viên liên hệ và thống nhất lại với bạn.</p>");
            html.AppendLine("<p>Chúng tôi sẽ xử lý đơn đặt hàng của bạn trong thời gian ngắn.</p>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }
    }



  

}

﻿using Application.DAL.Helper;
using Application.DAL.Models;
using AutoMapper;
using Core.Exceptions;
using Data.Contexts;
using Data.Repos.DiscountRepo;
using Data.Repos.OrderRepo;
using Data.Repos.ProductRepo;
using Data.UnitOfWork;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Models.DTOs.Order;
using Models.ResponseModels;
using Models.Status;
using Services.Interfaces;
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
        public OrderService(IUnitOfWork unitOfWork,
            IMapper mapper, 
            ApplicationDbContext context
            )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            
          
            _context = context;
        }

        public async Task<BaseResponse<OrderDto>> CreateOrder(OrderRequest request)
        {
            await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    OrderType = request.OrderType,
                    Address = request.Address,
                    Phone = request.Phone,
                    RecipientName = request.RecipientName,
                    SubTotal = request.SubTotal,
                    GrandTotal = request.GrandTotal,
                    Notes = request.Notes,
                    UserId = request.UserId,
                    Status = OrderStatus.NEWORDER, // Assuming a new order status
                    CreateAt = DateTime.UtcNow,
                    OrderItems = request.Items.Select(s =>
                    {
                        return new OrderItem
                        {
                            ProductItemId = s.ProductItemId,
                            Quantity = s.Quantity,
                            Price = s.Price
                        };
                    }).ToList(),
                    Transaction = new Transaction
                    {
                        Amount = request.Transaction.Amount,
                        Type = request.Transaction.Type,
                        Description = request.Transaction.Description,
                        UserId = request.Transaction.UserId,
                        Status = request.Transaction.Status
                    }
                 
                };
                // Tru san pham 
                foreach(var orderItem in request.Items)
                {
                    var product = await _unitOfWork.ProductRepository.GetProductItem(orderItem.ProductItemId);
                    product.Quantity -= orderItem.Quantity;
                    product.Product.ProductQuantity -= orderItem.Quantity;
                    if(product.Product.ProductQuantity < 0 || product.Product.ProductQuantity< 0) {
                        await _context.Database.RollbackTransactionAsync();
                        throw new ApiException($"Internal server error: Not enough product quantity")
                        { StatusCode = (int)HttpStatusCode.BadRequest };
                    }
                    await _unitOfWork.Repository<ProductItem>().Update(product);
                }
                
                order = await _unitOfWork.Repository<Order>().Insert(order);
                if(order == null)
                {
                    await _context.Database.RollbackTransactionAsync();
                    throw new ApiException($"Internal server error: Insert order failed")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }

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

        public async Task<BaseResponse<OrderResponse>> GetOrderDetail(Guid id)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.GetOrderDetail(id);
                if (order == null)
                {
                    throw new ApiException($"Not found")
                    { StatusCode = (int)HttpStatusCode.NotFound };
                }

                var response = new OrderResponse
                {
                    OrderId = order.Id,
                    OrderType = order.OrderType,
                    Address = order.Address,
                    Phone = order.Phone,
                    RecipientName = order.RecipientName,
                    SubTotal = order.SubTotal,
                    GrandTotal = order.GrandTotal,
                    Status = order.Status,
                    CreatedAt = order.CreateAt,
                    Notes = order.Notes,
                    OrderItems = order.OrderItems?.Select(oi => new OrderItemResponse
                    {
                        ProductItemId = oi.ProductItemId,
                        Quantity = oi.Quantity,
                        Price = oi.Price,
                        Product = oi.Product != null ? new ProductItemResponse
                        {
                            ProductId = oi.Product.ProductId,
                           
                            ProductName = oi.Product.Product.Name,
                            Color = oi.Product.Color != null ? new ColorResponse
                            {
                                ColorName = oi.Product.Color.ColorName,
                                ColorCode = oi.Product.Color.ColorCode
                            } : null,
                            ProductImages = oi.Product.ProductImages?.Select(pi => new ProductImageResponse
                            {
                                Url = pi.Url
                            }).ToList()
                        } : null
                    }).ToList(),
                    Transaction = order.Transaction != null ? new TransactionResponse
                    {
                        CreatedAt = order.Transaction.CreateAt,
                        Amount = order.Transaction.Amount,
                        Type = order.Transaction.Type,
                        Description = order.Transaction.Description,
                        Status = order.Transaction.Status
                    } : null
                };
                return new BaseResponse<OrderResponse>(response, "Order");
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
                    var discounts = await _unitOfWork.DiscountRepository.GetDiscountsByProductId(productItem.ProductId);
                    decimal bestDiscountAmount = 0;
                    var reviewCheckoutReviewItem = new ReviewCheckoutItem();

                    if(productItem == null) {
                        throw new ApiException($"Product item not found")
                        { StatusCode = (int)HttpStatusCode.NotFound };
                    }
                    if (!discounts.IsNullOrEmpty() && discounts.Any())
                    {
                        foreach (var discount in discounts)
                        {
                            if (discount.Discount.Status == DiscountStatus.ACTIVE && discount.Discount.DateStart <= DateTime.Now && discount.Discount.DateEnd >= DateTime.Now)
                            {
                                if(item.Quantity* productItem.Product.Price >= discount.Discount.MinimumPurchase)
                                {
                                    decimal discountAmount = 0;

                                    if (discount.Discount.Type == DiscountType.PERCENTAGE)
                                    {
                                        discountAmount = (productItem.Product.Price * item.Quantity * discount.Discount.DiscountValue) / 100;
                                    }
                                    else if (discount.Discount.Type == "fix-amount")
                                    {
                                        discountAmount = discount.Discount.DiscountValue;
                                    }

                                    if (discountAmount > bestDiscountAmount)
                                    {
                                        bestDiscountAmount = discountAmount;
                                    }
                                }    
                              
                            }
                            
                        }
                    }
                    reviewCheckoutReviewItem.ProductItemId = productItem.Id;
                    reviewCheckoutReviewItem.ProductName = productItem.Product.Name;
                    reviewCheckoutReviewItem.Quantity = item.Quantity;
                    reviewCheckoutReviewItem.Price = productItem.Product.Price;
                    reviewCheckoutReviewItem.AmountDiscount = bestDiscountAmount * item.Quantity;
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
            if(!OrderStatus.IsValidStatus(request.Status))
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
                    foreach(var orderItem in order.OrderItems)
                    {
                        var product = orderItem.Product;
                        product.Quantity += orderItem.Quantity;
                        product.Product.ProductQuantity += orderItem.Quantity;
                        await _unitOfWork.Repository<ProductItem>().Update(product);
                    }

                }

                order.Status = request.Status;
                order.UpdateAt = DateTime.UtcNow;
                order = await _unitOfWork.Repository<Order>().Update(order);
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
    }
}
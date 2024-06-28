﻿using Models.DTOs.Order;
using Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IOrderService
    {
        Task<BaseResponse<ReviewCheckoutResponse>> ReviewCheckoutOrder(ReviewCheckoutRequest request);
        Task<BaseResponse<OrderDto>> CreateOrder(OrderRequest request);
        Task<BaseResponse<ICollection<OrderDto>>> GetListOrder(int pageNumber, int pageSize);
        Task<BaseResponse<OrderDetailDto>> GetOrderDetail(Guid id);
        Task<BaseResponse<string>> UpdateStatus(OrderUpdateStatusRequest   request);
        Task<BaseResponse<ICollection<OrderDto>>> GetOrdersByUserId(string userId);
        Task SendMailOrder(Guid orderId);
        Task<(decimal totalRevenue, DateTime startDate, DateTime endDate)> GetTotalRevenueLastMonth();
        Task<bool> RemoveOrder(Guid orderId);
    }
}

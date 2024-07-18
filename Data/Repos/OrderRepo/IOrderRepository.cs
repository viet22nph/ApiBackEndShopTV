using Application.DAL.Models;
using Models.DTOs.Report;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.OrderRepo
{
    public interface IOrderRepository
    {

        Task<(ICollection<Order>, int)> GetOrders(int pageNumber, int pageSize);
        Task<Order> GetOrderDetail(Guid id);
        Task<ICollection<Order>> GetOrderByUserId(string userId);

        Task<bool> RemoveOrder(Guid orderId);
        Task<decimal> GetTotalRevenue(DateTime dateStart, DateTime dateEnd);
        Task<ICollection<DailyOrderSummary>> GetOrderSummary(DateTime startDate, DateTime endDate);
        Task<(ICollection<Order>, int)> GetListOrderByDate(DateTime date, int pageSize, int pageNumber);
    }
}

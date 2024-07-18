using Application.DAL.Models;
using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Identity.Client;
using Models.DTOs.Report;
using Models.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.OrderRepo
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Order>> GetOrderByUserId(string userId)
        {
            return await _context.Set<Order>()
          .Include(o => o.User)
          .Include(o => o.OrderItems)
              .ThenInclude(oi => oi.Product)
                  .ThenInclude(p => p.Color)
          .Include(o => o.OrderItems)
              .ThenInclude(oi => oi.Product)
                  .ThenInclude(p => p.ProductImages)
           .Include(o => o.OrderItems)
              .ThenInclude(oi => oi.Product)
                  .ThenInclude(p => p.Product)
          .Include(o => o.Transactions)
          .Where(o=> o.UserId == userId)
          .ToListAsync();
        }
        
        public async Task<Order> GetOrderDetail(Guid id)
        {
            return await _context.Set<Order>()
           .Include(o => o.User)
           .Include(o => o.OrderItems)
               .ThenInclude(oi => oi.Product)
                   .ThenInclude(p => p.Color)
           .Include(o => o.OrderItems)
               .ThenInclude(oi => oi.Product)
                   .ThenInclude(p => p.ProductImages)
            .Include(o => o.OrderItems)
               .ThenInclude(oi => oi.Product)
                   .ThenInclude(p => p.Product)
           .Include(o => o.Transactions)
           
           .FirstOrDefaultAsync(o => o.Id == id);
        }
        
        public async Task<(ICollection<Order>, int)> GetOrders(int pageNumber, int pageSize)
        {
            int count = await _context.Set<Order>().CountAsync();
            var orders = await _context.Set<Order>()
                        .Include(o=>o.OrderItems)
                        .Skip((pageNumber-1)* pageSize)
                        .Take(pageSize)
                        .OrderBy(p=> p.DateCreate)
                        //.GroupBy(p=> p.Status)
                        .ToListAsync();
            return (orders, count);
            
        }

        public async Task<decimal> GetTotalRevenue(DateTime dateStart, DateTime dateEnd)
        {
            var totalRevenue = await _context.Set<Order>()
            .Where(o => o.DateCreate >= dateStart && o.DateCreate <= dateEnd && o.Status == OrderStatus.COMPLETED)
            .SumAsync(o => o.Total);
            return totalRevenue;
        }

        public async Task<bool> RemoveOrder(Guid orderId)
        {
            var order = await _context.Set<Order>()
            .Include(o => o.OrderItems)
            .Include(o => o.Transactions)
            .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return false;
            }

            // Remove related entities
            if (order.OrderItems != null)
            {
                _context.Set<OrderItem>().RemoveRange(order.OrderItems);
            }

            if (order.Transactions != null)
            {
                _context.Set<Transaction>().RemoveRange(order.Transactions);
            }

            _context.Set<Order>().Remove(order);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<ICollection<DailyOrderSummary>> GetOrderSummary(DateTime startDate, DateTime endDate)
        {
            var summaries = new List<DailyOrderSummary>();

            // tính khoảng cách từ ngày bắt đầu đến ngày kết thúc
            var dateRange = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                      .Select(offset => startDate.AddDays(offset))
                                      .ToList();
            // lấy danh sách
            var orders = await _context.Set<Order>()
                                    .Include(o => o.OrderItems)
                                    .Where(o => o.DateCreate.Date >= startDate.Date && o.DateCreate <= endDate.Date)
                                    .ToListAsync();
            
            if (orders == null)
                return null;

            foreach (var date in dateRange)
            {
                var order = orders.Where(o => o.DateCreate.Date == date.Date).ToList();

                var dailySummary = new DailyOrderSummary
                {
                    Date = date,
                    TotalOrder = order.Count,
                    TotalProductSold = order.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity),
                    TotalRevenue = order.Sum(o => o.Total)
                };

                summaries.Add(dailySummary);
            }

            return summaries;
        }
        public async Task<(ICollection<Order>, int)> GetListOrderByDate(DateTime date, int pageSize, int pageNumber)
        {
            int count = await _context.Set<Order>().CountAsync();

            var orders =  await _context.Set<Order>()
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Color)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.ProductImages)
             .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Product)
            .Include(o => o.Transactions)
            .Where(p => p.DateCreate.Date == date.Date)
            .Skip((pageSize-1)* pageNumber)
            .Take(pageNumber)
            .ToListAsync();
            return (orders, count);

       
        }
    }
}

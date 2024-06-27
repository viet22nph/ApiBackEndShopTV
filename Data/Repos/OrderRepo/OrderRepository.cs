using Application.DAL.Models;
using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Identity.Client;
using Models.Status;
using System;
using System.Collections.Generic;
using System.Linq;
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
          .Include(o => o.Transaction)
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
           .Include(o => o.Transaction)
           
           .FirstOrDefaultAsync(o => o.Id == id);
        }
        
        public async Task<ICollection<Order>> GetOrders(int pageNumber, int pageSize)
        {
            var orders = await _context.Set<Order>()
                        .Include(o=>o.OrderItems)
                        .Skip((pageNumber-1)* pageSize)
                        .Take(pageSize)
                        .ToListAsync();
            return orders;
            
        }

        public async Task<decimal> GetTotalRevenue(DateTime dateStart, DateTime dateEnd)
        {
            var totalRevenue = await _context.Set<Order>()
            .Where(o => o.DateCreate >= dateStart && o.DateCreate <= dateEnd && o.Status == OrderStatus.COMPLETED)
            .SumAsync(o => o.GrandTotal);
            return totalRevenue;
        }

        public async Task<bool> RemoveOrder(Guid orderId)
        {
            var order = await _context.Set<Order>()
            .Include(o => o.OrderItems)
            .Include(o => o.Transaction)
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

            if (order.Transaction != null)
            {
                _context.Set<Transaction>().Remove(order.Transaction);
            }

            _context.Set<Order>().Remove(order);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

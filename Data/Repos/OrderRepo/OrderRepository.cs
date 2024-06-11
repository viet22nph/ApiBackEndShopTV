﻿using Application.DAL.Models;
using Data.Contexts;
using Microsoft.EntityFrameworkCore;
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
    }
}
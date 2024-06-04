using Application.DAL.Models;
using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.ProductRepo
{
#nullable disable
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<Product> GetProduct(Guid id)
        {
            // chưa lấy reviews
            var product = await _context.Set<Product>()
             .Include(p => p.ProductSpecifications)
          .Include(p => p.ProductItems)
              .ThenInclude(pi => pi.ProductImages)
          .Include(p => p.ProductItems)
              .ThenInclude(pi => pi.Color)
              .FirstOrDefaultAsync(p=> p.Id == id);
            return product;
        }

        public async Task<ProductItem> GetProductItem(Guid id)
        {
            var productItem = await _context.Set<ProductItem>()
                               .Include(pi=> pi.Product)
                               .FirstOrDefaultAsync(pi=>pi.Id == id);
            return productItem;
        }

        public async Task<ICollection<Product>> GetProducts(int pageNumber, int pageSize)
        {
           var product = await _context.Set<Product>()
               .Include(p => p.ProductSpecifications)
            .Include(p => p.ProductItems)
                .ThenInclude(pi => pi.ProductImages)
            .Include(p => p.ProductItems)
                .ThenInclude(pi => pi.Color)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            return product;
        }

        public async Task<ICollection<Product>> GetProductsIsDraft(int pageNumber, int pageSize)
        {
            var product = await _context.Set<Product>()
               .Include(p => p.ProductSpecifications)
            .Include(p => p.ProductItems)
                .ThenInclude(pi => pi.ProductImages)
            .Include(p => p.ProductItems)
                .ThenInclude(pi => pi.Color)
            .Skip((pageNumber - 1) * pageSize)
            .Where(p=> p.IsDraft == true)
            .Take(pageSize)
            .ToListAsync();
            return product;
        }

        public async Task<ICollection<Product>> GetProductsIsPublish(int pageNumber, int pageSize)
        {
            var product = await _context.Set<Product>()
               .Include(p => p.ProductSpecifications)
            .Include(p => p.ProductItems)
                .ThenInclude(pi => pi.ProductImages)
            .Include(p => p.ProductItems)
                .ThenInclude(pi => pi.Color)
            .Skip((pageNumber - 1) * pageSize)
            .Where(p => p.IsPublished == true)
            .Take(pageSize)
            .ToListAsync();
            return product;
        }
    }
}

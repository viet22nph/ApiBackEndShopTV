﻿using Application.DAL.Helper;
using Application.DAL.Models;
using Core.Extensions;
using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Models.Enums;
using Models.Status;
using System.Collections.Generic;
using static Models.Settings.Permissions;
using Category = Application.DAL.Models.Category;
using Product = Application.DAL.Models.Product;

namespace Data.Repos.ProductRepo
{
#nullable disable
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context
            ) {
            _context = context;
        }

        public async Task<Product> GetProduct(Guid id)
        {
            return await _context.Set<Product>()
                    .Include(p => p.ProductSpecifications)
                    .Include(p => p.ProductItems)
                        .ThenInclude(pi => pi.ProductImages)
                    .Include(p => p.ProductItems)
                        .ThenInclude(pi => pi.Color)
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .Include(p => p.Discount)
                    .FirstOrDefaultAsync(p=>p.Id == id);
        }

        public async Task<ProductItem> GetProductItem(Guid id)
        {
            var productItem = await _context.Set<ProductItem>()
                               .Include(pi=> pi.Product)
                                    .ThenInclude(p=> p.Category)
                                .Include(pi => pi.Product)
                                    .ThenInclude(p=> p.Discount)
                                .Include(pi => pi.Product)
                                    .ThenInclude(p => p.Supplier)
                                .Include(pi=> pi.ProductImages)
                                .Include(pi=>pi.Color)
                               .FirstOrDefaultAsync(pi=>pi.Id == id);
            return productItem;
        }

        public async Task<(ICollection<Product>, int)> GetProducts(int pageNumber, int pageSize)
        {
            int count = await _context.Set<Product>().CountAsync();
            var product = await _context.Set<Product>()
               .Include(p => p.ProductSpecifications)
            .Include(p => p.ProductItems)
                .ThenInclude(pi => pi.ProductImages)
            .Include(p => p.ProductItems)
                .ThenInclude(pi => pi.Color)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Include(p => p.Discount)
            .Skip((pageNumber - 1) * pageSize).Take(pageSize)
            .ToListAsync();
            return (product, count);
        }

        public async Task<(ICollection<Product>, int)> GetProductsIsDraft(int pageNumber, int pageSize)
        {
            var count = await _context.Set<Product>().Where(x=>x.IsDraft == true).CountAsync();
            var product = await _context.Set<Product>()
                .Include(p => p.ProductSpecifications)
             .Include(p => p.ProductItems)
                 .ThenInclude(pi => pi.ProductImages)
             .Include(p => p.ProductItems)
                 .ThenInclude(pi => pi.Color)
             .Include(p => p.Category)
             .Include(p => p.Supplier)
             .Include(p => p.Discount)
             .Where(p => p.IsPublished == false)
             .Skip((pageNumber - 1) * pageSize)
             .Take(pageSize)
             .ToListAsync();
            return (product, count);
        }
        public async Task<(ICollection<Product>, int)> GetProductIsPublicByPrice(decimal fromPrice, decimal toPrice, int pageNumber, int pageSize)
        {
            var count = await _context.Set<Product>().Where(x => x.IsPublished == true && x.Price >= fromPrice && x.Price <= toPrice).CountAsync();
            var product = await _context.Set<Product>()
             .Include(p => p.ProductSpecifications)
          .Include(p => p.ProductItems)
              .ThenInclude(pi => pi.ProductImages)
          .Include(p => p.ProductItems)
              .ThenInclude(pi => pi.Color)
          .Include(p => p.Category)
          .Include(p => p.Supplier)
          .Include(p => p.Discount)
          .Where(p => p.IsPublished == true && p.Price >= fromPrice && p.Price <= toPrice)
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();
            return (product, count);
        }
        public async Task<(ICollection<Product>,int )> GetProductsIsPublish(int pageNumber, int pageSize)
        {
            var count = await _context.Set<Product>().Where(x => x.IsPublished == true).CountAsync();
            var product = await _context.Set<Product>()
               .Include(p => p.ProductSpecifications)
            .Include(p => p.ProductItems)
                .ThenInclude(pi => pi.ProductImages)
            .Include(p => p.ProductItems)
                .ThenInclude(pi => pi.Color)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Include(p => p.Discount)
            .Where(p => p.IsPublished == true)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            return (product, count);
        }
        public async Task<(ICollection<Product>, int count)> GetNewProducts(int offset, int limit,bool isPublish = true)
        {
            int count = await _context.Set<Product>().CountAsync();
            var product = await _context.Set<Product>()
               .Include(p => p.ProductSpecifications)
            .Include(p => p.ProductItems)
                .ThenInclude(pi => pi.ProductImages)
            .Include(p => p.ProductItems)
                .ThenInclude(pi => pi.Color)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Include(p => p.Discount)
            .OrderByDescending(p=> p.DateCreate)
            .Where(p => p.IsPublished == isPublish)
            .Skip((offset - 1) * limit).Take(limit)
            .ToListAsync();

            return (product, count);
        }

        public async Task<ICollection<ProductItem>> TopSellingProduct(int top, DateTime dateStart, DateTime dateEnd)
        {
               var topProducts = await _context.Set<OrderItem>()
             .Include(oi => oi.Order)
             .Where(oi => oi.Order.DateCreate >= dateStart && oi.Order.DateCreate < dateEnd && oi.Order.Status == OrderStatus.COMPLETED)
             .GroupBy(oi => oi.ProductItemId)
             .Select(g => new 
             {
                 ProductItemId = g.Key,
                 TotalQuantitySold = g.Sum(oi => oi.Quantity)
             })
             .OrderByDescending(x => x.TotalQuantitySold)
             .Take(top)
             .ToListAsync();
            List< ProductItem > productItems = new List< ProductItem >();
            foreach(var item in topProducts)
            {
                productItems.Add(await GetProductItem(item.ProductItemId));
            }

            return productItems;
        }
        public async Task<List<Product>> GetProductsByCategoryPublish(Guid categoryId)
        {
            var category = await _context.Set<Category>()
                .Include(c => c.Products)
                .Include(c => c.CategoryChildren)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                return new List<Product>();
            }

            var products = new List<Product>();
            await CollectProducts(category,  products);
            return products;
        }
        public async Task<List<Product>> GetProductsByCategory(Guid categoryId)
        {
            var category = await _context.Set<Category>()
                .Include(c => c.Products)
                .Include(c => c.CategoryChildren)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                return new List<Product>();
            }

            var products = new List<Product>();
            await CollectProducts(category, products, false);
            return products;
        }
        private async Task CollectProducts(Category category,  List<Product> products, bool checkPublish = true)
        {

            if (category.CategoryChildren != null)
            {
                List<Product> data = new List<Product>();
                foreach (var childCategory in category.CategoryChildren)
                {
                    if (checkPublish)
                    {
                       data = await GetProductsByCategoryPublish(childCategory.Id);

                    }
                    else
                    {
                        data = await GetProductsByCategory(childCategory.Id);
                    } 
                    products.AddRange(data);
                }
            }

            if (category.Products != null)
            {
                if(checkPublish == true)
                {
                    foreach (var product in category.Products)
                    {
                        var rs = await GetProduct(product.Id);
                        if (rs.IsPublished == true)
                        {
                            products.Add(rs);

                        }
                    }
                }
                else
                {
                    foreach (var product in category.Products)
                    {
                        var rs = await GetProduct(product.Id);
                        products.Add(rs);

                        
                    }
                }
               
            }
        }

        public async Task<ICollection<Product>> QueryProductAsync(string query)
        {
            query = query.ToLower().RemoveDiacritics();
            // Start with the base query
            var productQuery = await _context.Set<Product>()
                .Include(p => p.ProductSpecifications)
                .Include(p => p.ProductItems)
                    .ThenInclude(pi => pi.ProductImages)
                .Include(p => p.ProductItems)
                    .ThenInclude(pi => pi.Color)
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Discount)
                .Where(p => p.IsPublished == true).ToListAsync();
             productQuery = productQuery.Where(p => p.Name.ToLower().RemoveDiacritics().Contains(query) || (p.Description?? "").Contains(query)).ToList();
               

            return productQuery;
        }
        
        public async Task<ResultRemoveItemEnums> RemoveProductItem(Guid id)
        {
            try
            {
                var productItem = _context.Set<ProductItem>().Include(p => p.ProductImages).Include(p => p.OrderItems).Include(p => p.Product).Where(p => p.Id == id).FirstOrDefault();
                if (productItem == null)
                {
                    return ResultRemoveItemEnums.NotFound;
                }
                if (productItem.OrderItems.Any())
                {
                    return ResultRemoveItemEnums.CanNotDelete;
                }
                var product = await GetProduct(productItem.Product.Id);
                if(product.ProductItems.Count <=1)
                { 
                    return ResultRemoveItemEnums.CanNotDelete;
                }    
                if (productItem.ProductImages != null)
                {
                    _context.Set<ProductImage>().RemoveRange(productItem.ProductImages);
                }
                var productItemQuantity = productItem.Quantity;
                productItem.Product.ProductQuantity -= productItemQuantity;
                _context.Set<ProductItem>().Remove(productItem);
                _context.SaveChanges();
                return ResultRemoveItemEnums.RemoveSuccess;
            }
            catch(Exception ex)
            {
                return ResultRemoveItemEnums.CanNotDelete;
            }
        }

        public async Task<(ICollection<Product>, int)> GetProductsOfTheSameCategory(Guid productId, int pageNumber, int pageSize )
        {
            var product = await _context.Set<Product>().FirstOrDefaultAsync(x => x.Id == productId);
            int count = await _context.Set<Product>().Where(x=> product.CategoryId   == x.CategoryId && x.Id != product.Id && x.IsPublished == true).CountAsync();
            var products = await _context.Set<Product>()
               .Include(p => p.ProductSpecifications)
            .Include(p => p.ProductItems)
                .ThenInclude(pi => pi.ProductImages)
            .Include(p => p.ProductItems)
                .ThenInclude(pi => pi.Color)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Include(p => p.Discount)
            .OrderByDescending(p => p.DateCreate)
            .Where(x => product.CategoryId == x.CategoryId && x.Id != product.Id && x.IsPublished == true)
            .Skip((pageNumber - 1) * pageSize).Take(pageSize)
            .ToListAsync();

            return (products, count);
        }
    }
}

using Application.DAL.Models;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.ProductRepo
{
    public interface IProductRepository
    {
        Task<(ICollection<Product>, int)> GetProducts(int pageNumber, int pageSize);

      Task<(ICollection<Product>, int)> GetProductsIsDraft(int pageNumber, int pageSize);

        Task<(ICollection<Product>, int)> GetProductsIsPublish(int pageNumber, int pageSize);
        Task<Product> GetProduct(Guid id);

        Task<ProductItem> GetProductItem(Guid id);
        Task<(ICollection<Product>, int count)> GetNewProducts(int offset, int limit, bool isPublish = true);
        Task<ICollection<ProductItem>> TopSellingProduct(int top, DateTime dateStart, DateTime dateEnd);
        Task<List<Product>> GetProductsByCategoryPublish(Guid categoryId);
        Task<List<Product>> GetProductsByCategory(Guid categoryId);
        Task<ICollection<Product>> QueryProductAsync(string query);
        Task<ResultRemoveItemEnums> RemoveProductItem(Guid id);
    }
}

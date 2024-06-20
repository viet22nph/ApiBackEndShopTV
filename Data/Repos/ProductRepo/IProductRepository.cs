using Application.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.ProductRepo
{
    public interface IProductRepository
    {
        Task<ICollection<Product>> GetProducts(int pageNumber, int pageSize);

        Task<ICollection<Product>> GetProductsIsDraft(int pageNumber, int pageSize);

        Task<ICollection<Product>> GetProductsIsPublish(int pageNumber, int pageSize);
        Task<Product> GetProduct(Guid id);

        Task<ProductItem> GetProductItem(Guid id);
        Task<(ICollection<Product>, int count)> GetNewProducts(int offset, int limit);
        Task<ICollection<ProductItem>> TopSellingProduct(int top, DateTime dateStart, DateTime dateEnd);
    }
}

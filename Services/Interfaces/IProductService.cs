using Models.DTOs.Product;
using Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IProductService
    {
        Task<(BaseResponse<ICollection<ProductResponse>>, int)> GetProducts(int pageNumber, int pageSize);
        Task<BaseResponse<ProductResponse>> GetProduct(Guid id);
        Task<BaseResponse<ProductDto>> CreateProduct(ProductRequest request);
        Task<(BaseResponse<ICollection<ProductResponse>>, int)> GetProductsIsDraft(int pageNumber, int pageSize);
        Task<(BaseResponse<ICollection<ProductResponse>>, int)> GetProductsIsPublish(int pageNumber, int pageSize);
        Task<BaseResponse<ProductDto>> UpdateProductPublish(Guid id);
        Task<BaseResponse<ProductDto>> UpdateProductDraft(Guid id);

        Task<BaseResponse<ProductDto>> UpdateProduct(Guid id, ProductUpdateRequest request);
        Task AddImage(Guid id, string url);
        Task<BaseResponse<ICollection<object>>> GetTopBestSellingProductsLastMonth(int top);
        Task<(BaseResponse<ICollection<ProductResponse>>, int count)> GetNewProducts(int limit, int offset);
        Task<(BaseResponse<ICollection<ProductResponse>>, int)> GetProductByCategory(Guid id, int limit, int offset);
        Task<(ICollection<ProductResponse>, int)> QueryProduct(string query, int limit, int offset);
        Task RemoveProductItem(Guid id);
        Task RemoveProductSpecification(Guid id);
    }
}

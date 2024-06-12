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
        Task<BaseResponse<ICollection<ProductResponse>>> GetProducts(int pageNumber, int pageSize);
        Task<BaseResponse<ProductResponse>> GetProduct(Guid id);
        Task<BaseResponse<ProductDto>> CreateProduct(ProductRequest request);

        Task<BaseResponse<ICollection<ProductResponse>>> GetProductsIsDraft(int pageNumber, int pageSize);

        Task<BaseResponse<ICollection<ProductResponse>>> GetProductsIsPublish(int pageNumber, int pageSize);
        Task<BaseResponse<ProductDto>> UpdateProductPublish(Guid id);
        Task<BaseResponse<ProductDto>> UpdateProductDraft(Guid id);

        Task<BaseResponse<ProductDto>> UpdateProduct(Guid id, ProductUpdateRequest request);
    }
}

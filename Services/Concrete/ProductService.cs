using Application.DAL.Models;
using AutoMapper;
using Caching;
using Core.Exceptions;
using Data.Contexts;
using Data.Repos.ProductRepo;
using Data.UnitOfWork;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Identity.Client;
using Models.DTOs.Product;
using Models.Models;
using Models.ResponseModels;
using Models.Status;
using Services.Interfaces;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApi.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Services.Concrete
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly ICacheManager _cacheManager;
        public ProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ApplicationDbContext context,
            ICacheManager cacheManager)
        { 
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
            _cacheManager = cacheManager;
        }

        public async Task AddImage(Guid id, string url)
        {
            var productItem = await _unitOfWork.Repository<ProductItem>().GetById(id);
            if(productItem == null)
            {
                throw new ApiException($"Internal server error: Not found") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            var image = new ProductImage { ProductItemId = id , Url = url};
            image = await _unitOfWork.Repository<ProductImage>().Insert(image);
        


            if (image == null)
            {
                throw new ApiException($"Internal server error: Add image is failed") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }

        public async Task<BaseResponse<ProductDto>> CreateProduct(ProductRequest request)
        {
            await _context.Database.BeginTransactionAsync();
            try
            {
                var product = _mapper.Map<Product>(request);
                product.NormalizedName = product.Name.ToUpper();
                product.Id = Guid.NewGuid();
                product = await _unitOfWork.Repository<Product>().Insert(product);
                await _context.Database.CommitTransactionAsync();
                var data = await _unitOfWork.ProductRepository.GetProduct(product.Id);
                var res = _mapper.Map<ProductDto>(data);
                
                return new BaseResponse<ProductDto>(res, "Create product successfully");
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            };
        }
        public async Task<BaseResponse<ProductResponse>> GetProduct(Guid id)
        {
            try
            {


                var product = await _unitOfWork.ProductRepository.GetProduct(id);
                if (product == null)
                {
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }
                var productDto = _mapper.Map<ProductResponse>(product);
                if (product?.ProductItems?.First()?.ProductImages.Count == 0)
                {
                    productDto.Image = "";

                }
                else
                {
                    productDto.Image = product.ProductItems.First().ProductImages.First().Url;
                }
                if (product.Discount == null)
                {
                    productDto.ProductDiscount = new ProductDiscount();
                }
                else
                {

                    productDto.ProductDiscount = new ProductDiscount();
                    if(product.Discount.Status == DiscountStatus.ACTIVE)
                    {
                        productDto.ProductDiscount.Value = product.Discount.DiscountValue;
                        productDto.ProductDiscount.Type = product.Discount.Type;

                    }    
                }
                // get rating
                productDto.Rating = new Rating();

                return new BaseResponse<ProductResponse>(productDto, "Product");
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }

        public async Task<BaseResponse<ICollection<ProductResponse>>> GetProducts(int pageNumber, int pageSize)
        {
            try
            {

                var products = await _unitOfWork.ProductRepository.GetProducts(pageNumber, pageSize);
                if(products == null)
                {
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }
                var productDtos = new List<ProductResponse>();
                foreach(var product in products)
                {
                    var productDto = _mapper.Map<ProductResponse>(product);
                    if (product?.ProductItems?.First()?.ProductImages.Count ==0)
                    {
                        productDto.Image ="";

                    }
                    else
                    {
                        productDto.Image = product.ProductItems.First().ProductImages.First().Url;
                    }

                    // get discount
                  
                    if(product.Discount == null)
                    {
                        productDto.ProductDiscount = new ProductDiscount();
                    }
                    else
                    {
                        productDto.ProductDiscount = new ProductDiscount();
                        if(product.Discount.Status != DiscountStatus.ACTIVE)
                        {
                            productDto.ProductDiscount.Value = product.Discount.DiscountValue;
                            productDto.ProductDiscount.Type = product.Discount.Type;
                        }    
                      
                    }
                    // get rating
                    var (Reviews, TotalCount, averageRating) = await _unitOfWork.ReviewRepository.GetReviewsByProductId(product.Id, 1, 0);
                    productDto.Rating = new Rating
                    {
                        Count = TotalCount,
                        Rate = averageRating
                    };

                    productDtos.Add(productDto);
                }
                return new BaseResponse<ICollection<ProductResponse>>(productDtos, "Products");
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }


        }

        public async Task<BaseResponse<ICollection<ProductResponse>>> GetProductsIsDraft(int pageNumber, int pageSize)
        {
            try
            {
                var products = await _unitOfWork.ProductRepository.GetProductsIsDraft(pageNumber, pageSize);
                if (products == null)
                {
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }
                var productDtos = new List<ProductResponse>();
                foreach (var product in products)
                {
                    var productDto = _mapper.Map<ProductResponse>(product);
                    if (product?.ProductItems?.First()?.ProductImages.Count == 0)
                    {
                        productDto.Image = "";

                    }
                    else
                    {
                        productDto.Image = product.ProductItems.First().ProductImages.First().Url;
                    }

                    // get discount

                    if (product.Discount == null)
                    {
                        productDto.ProductDiscount = new ProductDiscount();
                    }
                    else
                    {
                        productDto.ProductDiscount = new ProductDiscount();
                        if (product.Discount.Status == DiscountStatus.ACTIVE)
                        {
                            productDto.ProductDiscount.Value = product.Discount.DiscountValue;
                            productDto.ProductDiscount.Type = product.Discount.Type;
                        }
                      
                        
                    }
                    // get rating
                    var (Reviews, TotalCount, averageRating) = await _unitOfWork.ReviewRepository.GetReviewsByProductId(product.Id, 1, 1);
                    productDto.Rating = new Rating
                    {
                        Count = TotalCount,
                        Rate = averageRating
                    };
                    
                    productDtos.Add(productDto);
                    
                }
                return new BaseResponse<ICollection<ProductResponse>>(productDtos, "Products");
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }

        }

        public async Task<BaseResponse<ICollection<ProductResponse>>> GetProductsIsPublish(int pageNumber, int pageSize)
        {
            try
            {

                var products = await _unitOfWork.ProductRepository.GetProductsIsPublish(pageNumber, pageSize);
                if (products == null)
                {
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }
                var productDtos = new List<ProductResponse>();
                foreach (var product in products)
                {
                    var productDto = _mapper.Map<ProductResponse>(product);
                    if (product?.ProductItems?.First()?.ProductImages.Count == 0)
                    {
                        productDto.Image = "";

                    }
                    else
                    {
                        productDto.Image = product.ProductItems.First().ProductImages.First().Url;
                    }

                    // get discount

                    if (product.Discount == null)
                    {
                        productDto.ProductDiscount = new ProductDiscount();
                    }
                    else
                    {
                        productDto.ProductDiscount = new ProductDiscount();
                        if (product.Discount.Status != DiscountStatus.ACTIVE)
                        {
                            productDto.ProductDiscount.Value = product.Discount.DiscountValue;
                            productDto.ProductDiscount.Type = product.Discount.Type;
                        }
                    }
                    // get rating
                    var (Reviews, TotalCount, averageRating) = await _unitOfWork.ReviewRepository.GetReviewsByProductId(product.Id, 1, 0);
                    productDto.Rating = new Rating
                    {
                        Count = TotalCount,
                        Rate = averageRating
                    };
                    productDtos.Add(productDto);
                }
                return new BaseResponse<ICollection<ProductResponse>>(productDtos, "Products");
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }

        }


        public async Task<BaseResponse<ProductDto>> UpdateProduct(Guid id, ProductUpdateRequest request)
        {
            await _context.Database.BeginTransactionAsync();
            try
            {
                var product = await _unitOfWork.ProductRepository.GetProduct(id);
                if (product == null)
                {
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }
                EntityUpdater.UpdateIfNotNull(request.Name, value => product.Name = value);
                product.NormalizedName = product.Name.ToUpper();
                EntityUpdater.UpdateIfNotNull(request.Description, value => product.Description = value);
                EntityUpdater.UpdateIfNotNull(request.ProductQuantity, value => product.ProductQuantity = value);
                EntityUpdater.UpdateIfNotNull(request.ProductBrand, value => product.ProductBrand = value);
                EntityUpdater.UpdateIfNotNull(request.Price, value => product.Price = value);
                EntityUpdater.UpdateIfNotNull(request.SupplierId, value => product.SupplierId = value);
                EntityUpdater.UpdateIfNotNull(request.CategoryId, value => product.CategoryId = value);
                EntityUpdater.UpdateIfNotNull(request.DiscountId, value => product.DiscountId = value);

                // Update ProductSpecifications
                if (request.ProductSpecifications != null)
                {
                    foreach (var specRequest in request.ProductSpecifications)
                    {
                        if (specRequest.Id.HasValue)
                        {
                            var spec = product.ProductSpecifications?.FirstOrDefault(s => s.Id == specRequest.Id.Value);
                            if (spec != null)
                            {
                                EntityUpdater.UpdateIfNotNull(specRequest.SpecValue, value => spec.SpecValue = value);
                                EntityUpdater.UpdateIfNotNull(specRequest.SpecType, value => spec.SpecType = value);
                            }
                        }
                        else
                        {
                            product.ProductSpecifications?.Add(new ProductSpecification
                            {
                                SpecValue = specRequest.SpecValue,
                                SpecType = specRequest.SpecType
                            });
                        }
                    }
                }
                // Update ProductItems
                if (request.ProductItems != null)
                {
                    foreach (var itemRequest in request.ProductItems)
                    {
                        if (itemRequest.Id.HasValue)
                        {
                            var item = product.ProductItems?.FirstOrDefault(i => i.Id == itemRequest.Id.Value);
                            if (item != null)
                            {
                                EntityUpdater.UpdateIfNotNull(itemRequest.Quantity, value => item.Quantity = value);
                                EntityUpdater.UpdateIfNotNull(itemRequest.ColorId, value => item.ColorId = value);
                            }
                        }
                        else
                        {
                            var newItem = new ProductItem
                            {
                                Quantity = itemRequest.Quantity ?? 0,
                                ColorId = itemRequest.ColorId
                            };

                            // Add new ProductImages
                            product.ProductItems?.Add(newItem);
                        }
                    }
                }
                product = await _unitOfWork.Repository<Product>().Update(product);
                                var res = _mapper.Map<ProductDto>(product);
                return new BaseResponse<ProductDto>(res, "Product");
            }
            catch ( Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }

        public async Task<BaseResponse<ProductDto>> UpdateProductDraft(Guid id)
        {
            try
            {

                var product = await _unitOfWork.ProductRepository.GetProduct(id);
                if (product == null)
                {
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }
                product.IsDraft = true;
                product.IsPublished = false;
                product = await _unitOfWork.Repository<Product>().Update(product);
               
                var res = _mapper.Map<ProductDto>(product);
                return new BaseResponse<ProductDto>(res, "update success");

            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }

        public async Task<BaseResponse<ProductDto>> UpdateProductPublish(Guid id)
        {
            try
            {

                var product = await _unitOfWork.ProductRepository.GetProduct(id);
                if (product == null)
                {
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }
                product.IsDraft = false;
                product.IsPublished = true;
                product = await _unitOfWork.Repository<Product>().Update(product);
                
                var res = _mapper.Map<ProductDto>(product);
                return new BaseResponse<ProductDto>(res, "update success");

            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }


        // lấy top sản phẩm bán chạy
        public async Task<BaseResponse<ICollection<object>>> GetTopBestSellingProductsLastMonth(int top)
        {
            var lastMonth = DateTime.Now.AddMonths(-1);
            var startOfLastMonth = new DateTime(lastMonth.Year, lastMonth.Month, 1);
            var endOfLastMonth = startOfLastMonth.AddMonths(1).AddDays(-1);

            var productItems = await _unitOfWork.ProductRepository.TopSellingProduct(top, new DateTime(2024, 6, 1), new DateTime(2024, 6, 30));
            var result = new List<object>();
            if (productItems.Count == 0)
            {
                return new BaseResponse<ICollection<object>>(null, "Carts");
            }
            foreach ( var item in productItems)
            {
                result.Add(new
                {
                    ProductName = item?.Product.Name,
                    CategoryName = item.Product?.Category.Name,
                    Supplier = item.Product?.Supplier?.SupplierName ?? "",
                    Price = item.Product.Price,
                    
                    Discount =item.Product.Discount.Status == DiscountStatus.ACTIVE? new
                    {
                        type = item.Product.Discount.Type ?? null,
                        value = item.Product.Discount.DiscountValue
                        
                    }: null,
                    Image = item.ProductImages.First().Url ?? ""
                });
            }    
            return new BaseResponse<ICollection<object>>(result, $"Top {top} selling for {lastMonth} month");
        }

        public async Task<(BaseResponse<ICollection<ProductResponse>>, int count)> GetNewProducts(int limit, int offset)
        {
            try
            {
                var (products, count) = await _unitOfWork.ProductRepository.GetNewProducts(offset, limit);
                if (products == null)
                {
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }
                var productDtos = new List<ProductResponse>();
                foreach (var product in products)
                {
                    var productDto = _mapper.Map<ProductResponse>(product);
                    if (product?.ProductItems?.First()?.ProductImages.Count == 0)
                    {
                        productDto.Image = "";

                    }
                    else
                    {
                        productDto.Image = product.ProductItems.First().ProductImages.First().Url;
                    }

                    // get discount

                    if (product.Discount == null)
                    {
                        productDto.ProductDiscount = new ProductDiscount();
                    }
                    else
                    {
                        productDto.ProductDiscount = new ProductDiscount();
                        if (product.Discount.Status != DiscountStatus.ACTIVE)
                        {
                            productDto.ProductDiscount.Value = product.Discount.DiscountValue;
                            productDto.ProductDiscount.Type = product.Discount.Type;
                        }

                    }
                    // get rating
                    var (Reviews, TotalCount, averageRating) = await _unitOfWork.ReviewRepository.GetReviewsByProductId(product.Id, 1, 0);
                    productDto.Rating = new Rating
                    {
                        Count = TotalCount,
                        Rate = averageRating
                    };

                    productDtos.Add(productDto);
                }
                return (new BaseResponse<ICollection<ProductResponse>>(productDtos, "Products"), count);
            }
            catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
           
        }

        public async Task<(BaseResponse<ICollection<ProductResponse>>, int)> GetProductByCategory(Guid id, int limit, int offset)
        {
            try
            {
                int count = 0;

                var products = await _unitOfWork.ProductRepository.GetAllProductsByCategory(id);
                count = products.Count;
                products = products.Skip((offset-1)*limit).Take(limit).ToList();
                if (products == null)
                {
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }
                var productDtos = new List<ProductResponse>();
                foreach (var product in products)
                {
                    var productDto = _mapper.Map<ProductResponse>(product);
                    if (product?.ProductItems?.First()?.ProductImages.Count == 0)
                    {
                        productDto.Image = "";

                    }
                    else
                    {
                        productDto.Image = product.ProductItems.First().ProductImages.First().Url;
                    }

                    // get discount

                    if (product.Discount == null)
                    {
                        productDto.ProductDiscount = new ProductDiscount();
                    }
                    else
                    {
                        productDto.ProductDiscount = new ProductDiscount();
                        if (product.Discount.Status != DiscountStatus.ACTIVE)
                        {
                            productDto.ProductDiscount.Value = product.Discount.DiscountValue;
                            productDto.ProductDiscount.Type = product.Discount.Type;
                        }

                    }
                    // get rating
                    var (Reviews, TotalCount, averageRating) = await _unitOfWork.ReviewRepository.GetReviewsByProductId(product.Id, 1, 0);
                    productDto.Rating = new Rating
                    {
                        Count = TotalCount,
                        Rate = averageRating
                    };

                    productDtos.Add(productDto);
                }
                return (new BaseResponse<ICollection<ProductResponse>>(productDtos, "Products by category"), count);
            }
            catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };

            }
        }

        //public async Task<BaseResponse<ICollection<ProductDto>>> GetProductByCategory(Guid id)
        //{

        //}
    }
}

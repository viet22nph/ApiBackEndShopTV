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
using Models.Enums;
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

                        productDto.ProductDiscount.Id = product.Discount.Id;
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

        public async Task<(BaseResponse<ICollection<ProductResponse>>, int)> GetProducts(int pageNumber, int pageSize)
        {
            try
            {

                var (products,count) = await _unitOfWork.ProductRepository.GetProducts(pageNumber, pageSize);
                var res = await mapProductAsync(products);
                return (new BaseResponse<ICollection<ProductResponse>>(res, "Products"), count);
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }


        }

        public async Task<(BaseResponse<ICollection<ProductResponse>>, int)> GetProductsIsDraft(int pageNumber, int pageSize)
        {
            try
            {

                var (products, count) = await _unitOfWork.ProductRepository.GetProductsIsDraft(pageNumber, pageSize);
                var res = await mapProductAsync(products);
                return (new BaseResponse<ICollection<ProductResponse>>(res, "Products"), count);
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }

        }

        public async Task<(BaseResponse<ICollection<ProductResponse>>, int)> GetProductsIsPublish(int pageNumber, int pageSize)
        {
            try
            {


                var (products, count) = await _unitOfWork.ProductRepository.GetProductsIsPublish(pageNumber, pageSize);

                var res = await mapProductAsync(products);
                return (new BaseResponse<ICollection<ProductResponse>>(res, "Products"), count);
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

                // Update ProductSpecifications
                if (request.ProductSpecifications != null)
                {
                    var existingSpecs = product.ProductSpecifications.ToList();
                    foreach (var specRequest in request.ProductSpecifications)
                    {
                        if (specRequest.Id.HasValue)
                        {
                            var spec = existingSpecs.FirstOrDefault(s => s.Id == specRequest.Id.Value);
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

                    // Remove specifications not in the request
                    var specIdsToRemove = existingSpecs
                        .Where(s => !request.ProductSpecifications.Any(r => r.Id.HasValue && r.Id.Value == s.Id))
                        .ToList();

                    foreach (var spec in specIdsToRemove)
                    {
                        await RemoveProductSpecification(spec.Id);


                    }
                }
                // Update ProductItems
                if (request.ProductItems != null)
                {
                    var existingItems = product.ProductItems.ToList();
                    foreach (var itemRequest in request.ProductItems)
                    {
                        if (itemRequest.Id.HasValue)
                        {
                            var item = existingItems.FirstOrDefault(i => i.Id == itemRequest.Id.Value);
                            if (item != null)
                            {
                                EntityUpdater.UpdateIfNotNull(itemRequest.Quantity, value => item.Quantity = value);
                                EntityUpdater.UpdateIfNotNull(itemRequest.ColorId, value => item.ColorId = value);

                                if (itemRequest.ProductImages != null)
                                {
                                    var existingImages = item.ProductImages.ToList();
                                    foreach (var imageRequest in itemRequest.ProductImages)
                                    {
                                        if (imageRequest.Id.HasValue)
                                        {
                                            var image = existingImages.FirstOrDefault(img => img.Id == imageRequest.Id.Value);
                                            if (image != null)
                                            {
                                                EntityUpdater.UpdateIfNotNull(imageRequest.Url, value => image.Url = value);
                                            }
                                        }
                                        else
                                        {
                                            var newImage = new ProductImage
                                            {
                                                Url = imageRequest.Url
                                            };
                                            item.ProductImages?.Add(newImage);
                                        }
                                    }

                                    // Remove images not in the request
                                    var imageIdsToRemove = existingImages
                                        .Where(img => !itemRequest.ProductImages.Any(r => r.Id.HasValue && r.Id.Value == img.Id))
                                        .ToList();

                                    foreach (var image in imageIdsToRemove)
                                    {
                                        item.ProductImages?.Remove(image);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // add product id
                            var newItem = new ProductItem
                            {
                                Quantity = itemRequest.Quantity ?? 0,
                                ColorId = itemRequest.ColorId,
                                ProductImages = itemRequest?.ProductImages?.Select(i => new ProductImage
                                {
                                    Url = i.Url
                                }).ToList()
                            };
                            product.ProductItems?.Add(newItem);
                        }
                    }

                    // Remove items not in the request
                    var itemIdsToRemove = existingItems
                        .Where(i => !request.ProductItems.Any(r => r.Id.HasValue && r.Id.Value == i.Id))
                        .ToList();

                    foreach (var item in itemIdsToRemove)
                    {
                       var checkRemove = await _unitOfWork.ProductRepository.RemoveProductItem(item.Id);
                        if(checkRemove == ResultRemoveItemEnums.ConstraintOrder)
                        {
                            await _context.Database.RollbackTransactionAsync();
                            throw new ApiException($"Internal server error: Not remove product item id = {item.Id}") { StatusCode = (int)HttpStatusCode.BadRequest };

                        }    else if(checkRemove == ResultRemoveItemEnums.CanNotDelete)
                        {
                            await _context.Database.RollbackTransactionAsync();
                            throw new ApiException($"Internal server error: Not remove product item") { StatusCode = (int)HttpStatusCode.BadRequest };
                        }    
                        else if(checkRemove == ResultRemoveItemEnums.NotFound)
                        {
                            await _context.Database.RollbackTransactionAsync();
                            throw new ApiException($"Internal server error:Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                        }    
                    }
                }
                product = await _unitOfWork.Repository<Product>().Update(product);
                _context.Database.CommitTransaction();
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
                return new BaseResponse<ICollection<object>>(null, "Product selling");
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
                var res = await mapProductAsync(products);
                return (new BaseResponse<ICollection<ProductResponse>>(res, "Products"), count);
            }
            catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
           
        }
        public async Task RemoveProductItem(Guid id)
        {
            try
            {
                var checkRemove = await _unitOfWork.ProductRepository.RemoveProductItem(id);
                if (checkRemove == ResultRemoveItemEnums.ConstraintOrder)
                {
                    throw new ApiException($"Internal server error: Not remove product item id = {id}") { StatusCode = (int)HttpStatusCode.BadRequest };

                }
                else if (checkRemove == ResultRemoveItemEnums.CanNotDelete)
                {
                  
                    throw new ApiException($"Internal server error: Not remove product item") { StatusCode = (int)HttpStatusCode.BadRequest };
                }else if(checkRemove == ResultRemoveItemEnums.NotFound)
                {
                    throw new ApiException($"Internal server error: Not found") { StatusCode = (int)HttpStatusCode.NotFound };

                }


            }
            catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }
        public async Task RemoveProductSpecification(Guid id)
        {
            try
            {
                var productSpec = await _unitOfWork.Repository<ProductSpecification>().GetById(id);
                if(productSpec == null)
                {
                    throw new ApiException($"Internal server error: Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }
                var checkRemoveSuccess = await _unitOfWork.Repository<ProductSpecification>().Delete(productSpec);
                if(checkRemoveSuccess <=0 )
                {
                    throw new ApiException($"Internal server error: Can not delete product specification {id}") { StatusCode = (int)HttpStatusCode.BadRequest };
                }    
                

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

                var res = await mapProductAsync(products);
                return (new BaseResponse<ICollection<ProductResponse>>(res, "Products by category"), count);
            }
            catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };

            }
        }

        public async Task<(ICollection<ProductResponse>, int)> QueryProduct(string query, int limit, int offset)
        {
            try
            {
                int productCount = 0;

                ICollection<Product> products = await _unitOfWork.ProductRepository.QueryProductAsync(query);
                productCount = products.Count;
                products.Skip((offset -1)* limit).Take(limit);
                var res = await mapProductAsync(products);
                return (res, productCount);
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }

        

        private async Task<ICollection<ProductResponse>> mapProductAsync(ICollection<Product> products)
        {
            if (products == null)
            {
                throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
            }
            var productDtos = new List<ProductResponse>();
            foreach (var product in products)
            {
                var productDto = _mapper.Map<ProductResponse>(product);
                if (product?.ProductItems?.Count ==0 || product?.ProductItems?.First()?.ProductImages?.Count == 0)
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
                        productDto.ProductDiscount.Id = product.Discount.Id;
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
            return productDtos;
        }

        public async Task<BaseResponse<string>> updateProductDiscount(Guid productId, Guid? discountId)
        {
                var product = await _unitOfWork.Repository<Product>().GetById(productId);
                if(product == null)
                {
                    throw new ApiException($"Not found product") { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                if(discountId != null) {
                
                    var discount = await _unitOfWork.Repository<Discount>().GetById(discountId.Value);
                    if(discount == null)
                    {
                        throw new ApiException($"Not found discount with id = {discountId.Value}") { StatusCode = (int)HttpStatusCode.BadRequest };
                    }
                    product.DiscountId = discount.Id;
                }
                else
                {
                    product.DiscountId = null;
                }

                product = await _unitOfWork.Repository<Product>().Update(product);
                if(product == null)
                {
                    throw new ApiException($"Update product is failed with discount id ={productId} ") { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                return new BaseResponse<string>("Update success");

            

        }
    }
}

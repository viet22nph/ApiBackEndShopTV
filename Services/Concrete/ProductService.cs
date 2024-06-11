﻿using Application.DAL.Models;
using AutoMapper;
using Core.Exceptions;
using Data.Contexts;
using Data.Repos.ProductRepo;
using Data.UnitOfWork;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.DTOs.Product;
using Models.ResponseModels;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace Services.Concrete
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public ProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ApplicationDbContext context)
        { 
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
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
                return await GetProduct(product.Id);
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            };
        }
        public async Task<BaseResponse<ProductDto>> GetProduct(Guid id)
        {
            try
            {


                var product = await _unitOfWork.ProductRepository.GetProduct(id);
                if (product == null)
                {
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }

                var res = _mapper.Map<ProductDto>(product);
                return new BaseResponse<ProductDto>(res, "Product");
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }

        public async Task<BaseResponse<ICollection<ProductDto>>> GetProducts(int pageNumber, int pageSize)
        {
            try
            {

                var products = await _unitOfWork.ProductRepository.GetProducts(pageNumber, pageSize);
                if(products == null)
                {
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }
            
                var res = _mapper.Map<List<Product> , List<ProductDto>>((List<Product>)products);
                return new BaseResponse<ICollection<ProductDto>>(res, "Products");
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }


        }

        public async Task<BaseResponse<ICollection<ProductDto>>> GetProductsIsDraft(int pageNumber, int pageSize)
        {
            try
            {

                var products = await _unitOfWork.ProductRepository.GetProductsIsDraft(pageNumber, pageSize);
                if (products == null)
                {
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }

                var res = _mapper.Map<List<Product>, List<ProductDto>>((List<Product>)products);
                return new BaseResponse<ICollection<ProductDto>>(res, "Products");
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }

        }

        public async Task<BaseResponse<ICollection<ProductDto>>> GetProductsIsPublish(int pageNumber, int pageSize)
        {
            try
            {

                var products = await _unitOfWork.ProductRepository.GetProductsIsPublish(pageNumber, pageSize);
                if (products == null)
                {
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }

                var res = _mapper.Map<List<Product>, List<ProductDto>>((List<Product>)products);
                return new BaseResponse<ICollection<ProductDto>>(res, "Products");
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
                product.NormalizedName = request.Name.ToUpper();
                EntityUpdater.UpdateIfNotNull(request.Description, value => product.Description = value);
                EntityUpdater.UpdateIfNotNull(request.ProductQuantity, value => product.ProductQuantity = value);
                EntityUpdater.UpdateIfNotNull(request.ProductBrand, value => product.ProductBrand = value);
                EntityUpdater.UpdateIfNotNull(request.Price, value => product.Price = value);
                EntityUpdater.UpdateIfNotNull(request.SupplierId, value => product.SupplierId = value);
                EntityUpdater.UpdateIfNotNull(request.CategoryId, value => product.CategoryId = value);
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

    }
}
using Application.DAL.Models;
using AutoMapper;
using Caching;
using Core.Exceptions;
using Data.Contexts;
using Data.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.Cart;
using Models.DTOs.Discount;
using Models.DTOs.Product;
using Models.ResponseModels;
using Models.Status;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete
{
    public class CartService : ICartService
    {

        private readonly ICacheManager _cacheManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public CartService(ICacheManager cacheManager, IUnitOfWork unitOfWork, ApplicationDbContext context,IMapper mapper)
        {
            _cacheManager = cacheManager;
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        public async Task<BaseResponse<ICollection<object>>> AddToCart(CartRequest request)
        {
            var key = $"Cart:{request.UserId}";

            // lay tu cache
            var productItem = await _unitOfWork.Repository<ProductItem>().GetById(Guid.Parse(request.ProductItemId.ToString()));
            if(productItem == null)
            {
                throw new ApiException($"Internal server error: Product not found")
                { StatusCode = (int)HttpStatusCode.NotFound };
            }
            var user = await _context.Users.FirstOrDefaultAsync(x=> x.Id  == request.UserId);
            if (user == null)
            {
                throw new ApiException($"Internal server error: User id not found")
                { StatusCode = (int)HttpStatusCode.NotFound };
            }

            var quantityCart = await _cacheManager.GetHashAsync(key, request.ProductItemId.ToString());
            if(request.Quantity == null && request.IncrementBy == null)
            {
                throw new ApiException($"Internal server error: Quantity must have an input value")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            if (request.Quantity != null)
            {
                if (request.Quantity <= 0)
                {
                    throw new ApiException($"Internal server error: Quantity should be greater than 0")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                if (productItem.Quantity < request.Quantity)
                {
                    throw new ApiException($"Internal server error: Not enough products in stock")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                await _cacheManager.HashIncrementAsync(key, request.ProductItemId.ToString(),(long)request.Quantity-quantityCart);     
            }
            // increment 
            if(request.IncrementBy!= null)
            {
                if (request.IncrementBy != -1 && request.IncrementBy != 1)
                {
                    throw new ApiException($"Internal server error: Value of incrementBy should be 1 or -1")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                var quantityAfterIncrement = quantityCart + request.IncrementBy;
                if(quantityAfterIncrement > productItem.Quantity || quantityAfterIncrement <=0)
                {
                    throw new ApiException($"Internal server error: Can't decrement stock to 0")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                await _cacheManager.HashIncrementAsync(key, request.ProductItemId.ToString(),(long)request.IncrementBy);

            }
            return await GetCart(request.UserId);
        }

        public async Task<BaseResponse<ICollection<object>>> DeleteFromCart(string userId, Guid productItemId)
        {
            var key = $"Cart:{userId}";
            bool removed = await _cacheManager.RemoveHashAsync(key, productItemId.ToString());
            if (!removed)
            {

                throw new ApiException($"Internal server error: Remove is failed")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
                return await GetCart(userId);

        }

        public async Task<BaseResponse<ICollection<object>>> GetCart(string userId)
        {
            var key = $"Cart:{userId}";
            var data = await _cacheManager.HashGetAllAsync(key);
            var productItems= new List<object>();
            if (data.Count == 0)
            {
                return new BaseResponse<ICollection<object>>(null, "Carts");
             }
            foreach (var item in data)
            {
                var productItem = await _unitOfWork.ProductRepository.GetProductItem(Guid.Parse(item.Key));
                var productItemResponse = new
                {
                    id = productItem.Id,
                    name = productItem?.Product?.Name,
                    quantity = item.Value,
                    price = productItem.Product.Price,
                    image = productItem.ProductImages.Select(pi => new { id = pi.Id, url = pi.Url }).First(),
                    color = new { colorName = productItem.Color.ColorName, colorCode = productItem.Color.ColorCode },
                    discount = productItem.Product.Discount == null || productItem.Product.Discount.Status != DiscountStatus.ACTIVE
                    ? new ProductDiscount() 
                    :  new ProductDiscount { Type = productItem.Product.Discount.Type, Value = productItem.Product.Discount.DiscountValue },
                    quantityInStock = productItem.Quantity
                };
                productItems.Add(productItemResponse);
            }

            return new BaseResponse<ICollection<object>>(productItems, "Carts");

        }
    }
}

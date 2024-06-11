using Application.DAL.Models;
using Caching;
using Core.Exceptions;
using Data.Contexts;
using Data.UnitOfWork;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.Cart;
using Models.Models;
using Models.ResponseModels;
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
        public CartService(ICacheManager cacheManager, IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _cacheManager = cacheManager;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task AddToCart(CartRequest request)
        {
            var key = $"Cart:{request.UserId}";
            var productItem = await _unitOfWork.Repository<ProductItem>().GetById(request.ProductItemId);
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
            if(productItem.Quantity < request.Quantity)
            {
                throw new ApiException($"Internal server error: Product quantity does not match")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }    
            await _cacheManager.HashIncrementAsync(key, request.ProductItemId.ToString(), request.Quantity);
        }

        public async Task<BaseResponse<string>> DeleteFromCart(string userId, Guid productItemId)
        {
            var key = $"Cart:{userId}";
            bool removed = await _cacheManager.RemoveHashAsync(key, productItemId.ToString());
            if(removed)
            {
                return new BaseResponse<string>("Item removed from cart.");
            }
            else
            {
                throw new ApiException($"Internal server error: Item not found in cart.") { StatusCode = (int)HttpStatusCode.NotFound };
            }
        }

        public async Task<BaseResponse<CartDto>> GetCart(string userId)
        {
            var key = $"Cart:{userId}";
            var data = await _cacheManager.HashGetAllAsync(key);
            var cartDto = new CartDto()
            {
                UserId = userId,
                CartItems = new List<CartItem>()
            };
            foreach(var item in data )
            {
                cartDto.CartItems.Add(new CartItem
                {
                    ProductId = Guid.Parse(item.Key),
                    Quantity = (int)item.Value
                });
            }    
            return new BaseResponse<CartDto>(cartDto, "Cart");

        }
    }
}

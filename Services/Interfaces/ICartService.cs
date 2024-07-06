using Models.DTOs.Cart;
using Models.DTOs.Product;
using Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICartService
    {
        Task<BaseResponse<ICollection<object>>> AddToCart(CartRequest request);
        Task<BaseResponse<ICollection<object>>> GetCart(string userId);
        Task<BaseResponse<ICollection<object>>> DeleteFromCart(string userId, Guid productItemId);
    }
}

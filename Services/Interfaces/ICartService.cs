using Models.DTOs.Cart;
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
        Task AddToCart(CartRequest request);
        Task<BaseResponse<CartDto>> GetCart(string userId);
        Task<BaseResponse<string>> DeleteFromCart(string userId, Guid productItemId);
    }
}

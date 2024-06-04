using Models.DTOs.Account;
using Models.DTOs.User;
using Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService
    {

        Task<BaseResponse<ICollection<UserDto>>> GetUsers(int pageNumber =1 , int pageSize =10);
    }
}

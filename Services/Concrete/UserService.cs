using AutoMapper;
using Core.Exceptions;
using Data.Repos.UserRepo;
using Data.UnitOfWork;
using Models.DTOs.Account;
using Models.DTOs.User;
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
    public class UserService : IUserService
    {
        //private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserService( IMapper mapper,IUnitOfWork unitOfWork)
        {            
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<ICollection<UserDto>>> GetUsers(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var users = await _unitOfWork.UserRepository.GetUsers(pageNumber, pageSize);
                if(users == null)
                {
                    throw new ApiException($"Internal server error: Not found users")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                var res = _mapper.Map<List<UserDto>>(users);
                return new BaseResponse<ICollection<UserDto>>(res, "Users");
                
            }catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }

     
    }
}

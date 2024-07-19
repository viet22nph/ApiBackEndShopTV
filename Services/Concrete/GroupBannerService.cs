using AutoMapper;
using Core.Exceptions;
using Data.UnitOfWork;
using Microsoft.Identity.Client;
using Models.DTOs.GroupBanner;
using Models.Models;
using Models.ResponseModels;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete
{
    public class GroupBannerService : IGroupBannerService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GroupBannerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<GroupBannerDetailDto>> GetDetailGroupBannerAsync(Guid id)
        {
            try
            {
                var groupBanner = await _unitOfWork.GroupBannerRepository.GetDetailGroupBannerAsync(id);
                if(groupBanner == null)
                {
                    throw new ApiException($"Internal server error: Not found group banner id = {id}")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                if( groupBanner.Banners == null ||groupBanner?.Banners?.Count == 0)
                {
                    groupBanner.Banners = [];
                }
                var groupBannerDto = _mapper.Map<GroupBannerDetailDto>(groupBanner);
                return new BaseResponse<GroupBannerDetailDto>(groupBannerDto, "Banner detail");
               
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }

        public async Task<BaseResponse<ICollection< GroupBannerDto>>> GetGroupBannerAsync()
        {
            try
            {

                var groupBanner = await _unitOfWork.Repository<GroupBanner>().GetAll();
                if (groupBanner == null)
                {
                    return new BaseResponse<ICollection<GroupBannerDto>> { Data = [], Message = "Groups banner" };
                }

                var groupBannerDto = _mapper.Map<List<GroupBanner>, List<GroupBannerDto>>(groupBanner);
                return new BaseResponse<ICollection<GroupBannerDto>> { Data = groupBannerDto, Message = "Groups banner" };
            }catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }

        }

        public async Task<BaseResponse<GroupBannerDto>> ToogleEnableAsync(Guid id)
        {

            try
            {

                var groupBanner = await _unitOfWork.Repository<GroupBanner>().GetById(id);
                if (groupBanner == null)
                {
                    throw new ApiException($"Internal server error: Not found group banner id = {id}")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                groupBanner.IsEnable = groupBanner.IsEnable ? false : true;
                var groupBannerUpdate = await _unitOfWork.Repository<GroupBanner>().Update(groupBanner);
                if(groupBannerUpdate == null)
                {
                    throw new ApiException($"Internal server error: Update group banner {groupBanner.GroupName} is failed")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                var groupBannerDto = _mapper.Map<GroupBannerDto>(groupBanner);
                return new BaseResponse<GroupBannerDto> { Data = groupBannerDto, Message = $"Update group banner {groupBanner.GroupName} successfully" };
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }
    }
}

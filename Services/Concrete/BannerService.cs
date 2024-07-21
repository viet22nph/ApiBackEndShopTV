using AutoMapper;
using Core.Exceptions;
using Data.UnitOfWork;
using Models.DTOs.Banner;
using Models.DTOs.Banner.Request;
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
    public class BannerService : IBannerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BannerService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<BaseResponse<BannerDto>> CreateBannerServiceAsync(BannerRequestDto payload)
        {
            try
            {
                // check payload
              
                
                var groupBanner = await _unitOfWork.Repository<GroupBanner>().GetById(payload.GroupId);
                if (groupBanner == null)
                {

                    throw new ApiException($"Internal server error: Not found group banner id ={payload.GroupId}")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                // map banner


                var banner = new Banner
                {
                    GroupId = payload.GroupId,
                    Url = payload.Url,
                    IsEnable = payload.IsEnable,
                };

                // add banner
                var resultInsert = await _unitOfWork.Repository<Banner>().Insert(banner);
                if (resultInsert == null)
                {
                    throw new ApiException($"Internal server error: Insert banner is failed")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                // mapp dto
                var bannerDto = _mapper.Map<BannerDto>(resultInsert);
                return new BaseResponse<BannerDto>(bannerDto, "Create banner success");
            }
            catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }

        }

        public async Task<BaseResponse<ICollection<BannerDto>>> GetBannersAsync(int pageNumber, int pageSize)
        {
            try
            {
                var banners = await _unitOfWork.BannerRepository.GetBannersAsync(pageNumber, pageSize);
                if (banners == null)
                {
                    return new BaseResponse<ICollection<BannerDto>>([], "Banners");
                }
                var bannersDto = _mapper.Map<List<Banner>,List<BannerDto>>((List<Banner>)banners);
                return new BaseResponse<ICollection<BannerDto>>(bannersDto, $"Banners");
            }
            catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
          
        }

        public async Task<bool> RemoveBanner(Guid id)
        {
            try
            {
                var banner = await _unitOfWork.Repository<Banner>().GetById(id);
                if (banner == null)
                {
                    new ApiException($"Internal server error: Not found banner id = {id}")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                var checkRemove = await _unitOfWork.Repository<Banner>().Delete(banner);
                if (checkRemove <= 0)
                {
                    return false;
                }
                return true;

            }
            catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
      
        }

        public async Task<BaseResponse<BannerDto>> ToogleEnableAsync(Guid id)
        {
            try
            {
                var banner = await _unitOfWork.Repository<Banner>().GetById(id);
                if (banner == null)
                {

                    throw new ApiException($"Internal server error: Not found banner id ={id}")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }

                banner.IsEnable = banner.IsEnable ? false : true;
                var bannerUpdate= await _unitOfWork.Repository<Banner>().Update(banner);
                var groupBanner  = await _unitOfWork.Repository<GroupBanner>().GetById(bannerUpdate.GroupId);
                if (bannerUpdate == null)
                {
                    throw new ApiException($"Internal server error: Update banner id = {id} is failed")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                bannerUpdate.Group = groupBanner;
                var bannerDto = _mapper.Map<BannerDto>(bannerUpdate);
                return new BaseResponse<BannerDto>(bannerDto, $"Update banner id = {id} success");
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }
    }
}

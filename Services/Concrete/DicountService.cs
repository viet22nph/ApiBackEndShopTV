﻿using Application.DAL.Models;
using AutoMapper;
using Core.Exceptions;
using Data.Repos.DiscountRepo;
using Data.UnitOfWork;
using Models.DTOs.Discount;
using Models.ResponseModels;
using Models.Status;
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
    public class DicountService : IDiscountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DicountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }



        public async Task<BaseResponse<DiscountDto>> CancelledDiscountStatus(Guid id)
        {
            try
            {
                var discount = await _unitOfWork.Repository<Discount>().GetById(id);
                if (discount == null)
                {
                    throw new ApiException($"Not found")
                    { StatusCode = (int)HttpStatusCode.NotFound };
                }

                discount.Status = DiscountStatus.CANCELLED;
                discount = await _unitOfWork.Repository<Discount>().Update(discount);
                var res = _mapper.Map<DiscountDto>(discount);
                return new BaseResponse<DiscountDto>(res, "Update successfully");
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }

        public async Task<BaseResponse<DiscountDto>> CreateDiscount(DiscountRequest request)
        {
            
            if(request.DateStart < DateTime.Now)
            {
                throw new ApiException("Date time start must be greater than to date time now")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            if (request.DateEnd < DateTime.Now)
            {
                throw new ApiException("Date time end must be greater than to date time now")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            if(request.DateEnd < request.DateStart)
            {
                throw new ApiException("Date time end must be greater than to date time start")
                { StatusCode = (int)HttpStatusCode.BadRequest };

            }
            try
            {
                var discount = _mapper.Map<Discount>(request);
                discount.Status = DiscountStatus.PENDING;
                discount = await _unitOfWork.Repository<Discount>().Insert(discount);
                if (discount == null)
                {
                    throw new ApiException($"Internal server error: Insert discount failed")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }

                var res = _mapper.Map<DiscountDto>(discount);
                return new BaseResponse<DiscountDto>(res, "Insert discount success");
            }catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }

        public async Task<BaseResponse<DiscountDto>> GetDiscount(Guid id)
        {
            try
            {
                var discount  = await _unitOfWork.Repository<Discount>().GetById(id);
                if (discount == null)
                {
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }
                var res = _mapper.Map<DiscountDto>(discount);
                return new BaseResponse<DiscountDto>(res, "Discount");
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }

        public async Task<BaseResponse<ICollection<DiscountDto>>> GetDiscounts(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
               var discount = await _unitOfWork.DiscountRepository.GetDiscounts(pageNumber, pageSize);
               if(discount == null || discount.Count <1)
                { 
                    throw new ApiException($"Not found")
                    { StatusCode = (int)HttpStatusCode.NotFound };
                }    

               var res = _mapper.Map<List<DiscountDto>>(discount);

                return new BaseResponse<ICollection<DiscountDto>>(res, "Discounts");

            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }

        public async Task<BaseResponse<DiscountDto>> PauseDiscountStatus(Guid id)
        {
            try
            {
                var discount = await _unitOfWork.Repository<Discount>().GetById(id);
                if(discount == null)
                {
                    throw new ApiException($"Not found")
                    { StatusCode = (int)HttpStatusCode.NotFound };
                }
                if(discount.Status != DiscountStatus.ACTIVE)
                {

                    throw new ApiException($"Discount is not active yet")
                    { StatusCode = (int)HttpStatusCode.NotFound };
                }
                discount.Status = DiscountStatus.PAUSE;
                discount = await _unitOfWork.Repository<Discount>().Update(discount);
                var res = _mapper.Map<DiscountDto>(discount);
                return new BaseResponse<DiscountDto>(res, "Update successfully");
            }
            catch(Exception ex) 
            {
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }
        public async Task UpdateAllDiscountStatus()
        {
           // get all discount 

            var discounts = await _unitOfWork.Repository<Discount>().GetAll();
            foreach(var discount in discounts)
            {
                var currentDate = DateTime.Now;
                if (currentDate > discount.DateStart)
                {
                    discount.Status = DiscountStatus.PENDING;
                }
                else if (currentDate >= discount.DateStart && currentDate <= discount.DateEnd)
                {
                    discount.Status = DiscountStatus.ACTIVE;
                }
                else if (currentDate > discount.DateEnd)
                {
                    discount.Status = DiscountStatus.EXPIRED;
                }
                await _unitOfWork.Repository<Discount>().Update(discount);
            }    
            
        }

        public async Task<BaseResponse<DiscountDto>> UpdateDateTime(Guid id,DiscountDateTimeRequest request)
        {

            try
            {
                var discount = await _unitOfWork.Repository<Discount>().GetById(id);
                if (discount == null)
                {
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }
                if(request.DateStart.HasValue && request.DateEnd.HasValue)
                {
                   if(request.DateEnd < request.DateStart)
                   {

                        throw new ApiException("Date time end must be greater than to date time start")
                        { StatusCode = (int)HttpStatusCode.BadRequest };

                    }
                    else
                    {
                        discount.DateStart = (DateTime)request.DateStart;
                        discount.DateEnd = (DateTime)request.DateEnd;
                    }
                }
                else
                {
                    if (request.DateStart.HasValue && request.DateStart > discount.DateEnd)
                    {
                        throw new ApiException("Date time end must be greater than to date time start")
                        { StatusCode = (int)HttpStatusCode.BadRequest };
                    }
                    else
                    {

                        discount.DateStart = (DateTime)request.DateStart;
                    }

                    if (request.DateStart.HasValue && request.DateEnd < discount.DateStart)
                    {
                        throw new ApiException("Date time end must be greater than to date time start")
                        { StatusCode = (int)HttpStatusCode.BadRequest };
                    }
                    else
                    {

                        discount.DateEnd = (DateTime)request.DateEnd;
                    }
                }
                discount = await _unitOfWork.Repository<Discount>().Update(discount);
                if (discount == null) {
                    throw new ApiException($"Update error")
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                var res = _mapper.Map<DiscountDto>(discount);
                return new BaseResponse<DiscountDto>(res, "Update success");
                    
            } catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
          
        }
    }
}

using Application.DAL.Models;
using AutoMapper;
using Caching;
using Core.Exceptions;
using Data.Contexts;
using Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.Review;
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
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly ICacheManager _cacheManager;
        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper, ApplicationDbContext context, ICacheManager cacheManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
            _cacheManager = cacheManager;
        }
        public async Task<BaseResponse<ReviewResponse>> CreateReview(ReviewRequest request)
        {
            await _context.Database.BeginTransactionAsync();
            try
            {

                var review = _mapper.Map<Review>(request);
                var product = await _unitOfWork.Repository<Product>().GetById(review.ProductId);
                if(product == null)
                {
                    _context.Database.RollbackTransaction();
                    throw new ApiException($"Internal server error: Product not found") { StatusCode = (int)HttpStatusCode.BadRequest };
                }    
                review = await _unitOfWork.Repository<Review>().Insert(review);
                if(review == null)
                {
                    _context.Database.RollbackTransaction();
                    throw new ApiException($"Internal server error: Create review failed") { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                await _context.Database.CommitTransactionAsync();

                var res = _mapper.Map<ReviewResponse>(review);
                
                return new BaseResponse<ReviewResponse>(res, "Create review success");

            }catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }


        public async Task<(BaseResponse<ICollection<ReviewDto>> Reviews, int TotalCount, float averageRating)> GetReviewsByProductId(Guid productId, int pageNumber, int pageSize)
        {
            var product = await _unitOfWork.Repository<Product>().GetById(productId);
            if (product == null)
            {
                throw new ApiException($"Internal server error: Product not found") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            var (reviews, total , averageRating) = await _unitOfWork.ReviewRepository.GetReviewsByProductId(productId, pageNumber, pageSize);
            var reviewDto = reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                Rating = r.Rating,
                ReviewImages = r.ReviewImages.Count> 0? r.ReviewImages.Select(ri =>new ReviewImageResponse { Id = ri.Id, Url = ri.Url}).ToList(): [],
                CreateAt = r.DateCreate,
                Content = r.Content,
                UserName = string.IsNullOrEmpty(r.UserId) ? "Ẩn danh" : r.User.UserName
            })
            .ToList();
            _cacheManager.RemoveByPrefix("api/Product");
            return (new BaseResponse<ICollection<ReviewDto>>(reviewDto, "Reivews"), total, averageRating);

        }

        public async Task RemoveReview(Guid id)
        {
            var review = await _unitOfWork.Repository<Review>().GetById(id);
            if (review == null)
            {
                throw new ApiException($"Internal server error: Review not found")
                {
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }
             var response = await _unitOfWork.Repository<Review>().Delete(review);
            if(response <=0)
            {

                throw new ApiException($"Internal server error: Remove review id failed")
                {
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }    

        }
    }
}

using Models.DTOs.Review;
using Models.Models;
using Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IReviewService
    {
        Task<BaseResponse<ReviewResponse>> CreateReview (ReviewRequest request);
        Task RemoveReview(Guid id);
        Task<(BaseResponse< ICollection<ReviewDto>> Reviews, int TotalCount, float averageRating)> GetReviewsByProductId(Guid productId, int pageNumber, int pageSize);
    }
}

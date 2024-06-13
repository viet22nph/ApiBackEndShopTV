using Models.DTOs.Review;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.ReviewRepo
{
    public interface IReviewRepository
    {
        Task<(List<Review> Reviews, int TotalCount, float averageRating)> GetReviewsByProductId(Guid productId, int pageNumber, int pageSize);
    }
}

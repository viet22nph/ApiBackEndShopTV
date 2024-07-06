using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.ReviewRepo
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;
        public ReviewRepository(ApplicationDbContext context) {
            _context = context;
        }
        public async Task<(List<Review> Reviews, int TotalCount, float averageRating)> GetReviewsByProductId(Guid productId, int pageNumber, int pageSize)
        {
            var query = await _context.Set<Review>()
          .Where(r => r.ProductId == productId)
          .Include(r => r.Product)
          .Include(r => r.ReviewImages)
          .Include(r=> r.User)
          .ToListAsync();
         

            var totalCount =  query.Count;
            var averageRating = (float)0.0;
            if (totalCount != 0)
            {

                 averageRating = (float)query.Average(r => r.Rating);
            }
            var reviews =  query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).ToList();
            return (reviews, totalCount, averageRating);

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Review
{
    public class ReviewResponse
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string? UserId { get; set; }
        public double Rating { get; set; }
        public string Content { get; set; }
        public ICollection<ReviewImageResponse>? ReviewImages { get; set; }

    }
    public class ReviewImageResponse
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
    }
}

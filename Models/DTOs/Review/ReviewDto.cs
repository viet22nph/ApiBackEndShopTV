using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Review
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public double Rating { get; set; }
        public ICollection<ReviewImageResponse>? ReviewImages { get; set; }
        public DateTime CreateAt { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
    }
}

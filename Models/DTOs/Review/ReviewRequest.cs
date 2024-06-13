using Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Review
{
    public class ReviewRequest
    {
        [Required(ErrorMessage = "Product id is required")]
        public Guid ProductId { get; set; }
        public string? UserId { get; set; }
        [Required(ErrorMessage = "Rating is required")]
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
        public int Rating { get; set; }
        [Required(ErrorMessage = "Content is required")]
        [StringLength(1000, ErrorMessage = "Content must be less than 1000 characters")]
        public string Content { get; set; }
        public virtual ICollection<ReviewImageRequest>? ReviewImages { get; set; }

    }
    public class ReviewImageRequest
    {
        public string Url { get; set; }
    }
}

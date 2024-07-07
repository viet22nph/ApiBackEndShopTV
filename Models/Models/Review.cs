using Application.DAL.Models;
using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Review : BaseEntity
    {
        public Guid ProductId { get; set; }
        public string? UserId { get; set; }
        public double Rating { get; set; }
        public string Content { get; set; }
        [JsonIgnore]
        public virtual Product? Product { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser? User { get; set; }
        [JsonIgnore]
        public virtual ICollection<ReviewImage>? ReviewImages { get; set; }
    }

    public class ReviewImage:BaseEntity
    {
        public Guid ReviewId { get; set; }
        public String Url { get; set; }
        [JsonIgnore]
        public virtual Review? Review { get; set; }
    }
}

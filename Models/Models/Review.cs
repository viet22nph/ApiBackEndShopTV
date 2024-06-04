using Application.DAL.Models;
using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Review : BaseEntity
    {

        public Guid ProductId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }
        public string? content { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual Product? Product { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}

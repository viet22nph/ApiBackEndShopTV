
using Application.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace Models.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string? Address { get; set; }
        public string? FullName { get; set; }
        public string DislayName { get; set; }

        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }


        public virtual ICollection<Order>? Order { get; set; }

        public virtual ICollection<Blog>? Blogs { get; set; }

    }
}

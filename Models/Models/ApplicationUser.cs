
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
        [JsonIgnore]

        public virtual ICollection<Review>? Reviews { get; set; }
        [JsonIgnore]

        public virtual ICollection<Order>? Order { get; set; }

    }
}

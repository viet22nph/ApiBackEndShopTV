using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class ContactUs: BaseEntity
    {
        public string FullName { get; set; }
        public string  Email { get; set; }
        public string Phone { get; set; }
        public string ContactContent { get; set; }

        public bool IsReply { get; set; } = false;
    }
}

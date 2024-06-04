using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Supplier
{
    public class SupplierRequest
    {
        [Phone(ErrorMessage = "Contact phone malformed")]
        public string? ContactPhone { get; set; }

        [Required(ErrorMessage = "Supplier contact person not null or white space")]
        public string ContactPerson { get; set; }

        [Required(ErrorMessage = "Supplier name not null or white space")]
        public string SupplierName { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
    }
}

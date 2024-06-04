using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Supplier
{
    public class SupplierDto
    {
        public Guid Id { get; set; }
        public string? ContactPhone { get; set; }
        public string ContactPerson { get; set; }
        public string SupplierName { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
    }
}

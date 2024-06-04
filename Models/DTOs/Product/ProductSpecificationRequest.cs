using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Product
{
    public class ProductSpecificationRequest
    {
        [Required(ErrorMessage = "SpecValue not null or empty")]
        //[RegularExpression(@"^[a-zA-Z0-9\s\-_/*^()~%$+''"",.|]*$", ErrorMessage = "Product name cannot contain special characters")]
        public string SpecValue { get; set; }
        [Required(ErrorMessage = "SpecValue not null or empty")]
        //[RegularExpression(@"^[a-zA-Z0-9\s\-_/*^()~%$+''"",.|]*$", ErrorMessage = "Product name cannot contain special characters")]
        public string SpecType { get; set; }
    }
}

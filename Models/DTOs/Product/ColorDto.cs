using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Product
{
    public class ColorDto
    {
        public Guid Id { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
    }
}

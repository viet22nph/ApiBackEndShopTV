using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Discount
{
    public class DiscountDateTimeRequest
    {

        public DateTime? DateStart { get; set; }// 11/11/2024
        public DateTime? DateEnd { get; set; }//11/12/2024
    }
}

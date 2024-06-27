using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestModels
{
    public class VnpayRequest
    {
        public Guid? OrderId { get; set; }
        public String? UserId { get; set; }
        public decimal? Amount { get; set; }

    }
}

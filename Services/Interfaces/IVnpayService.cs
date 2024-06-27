using Microsoft.AspNetCore.Http;
using Models.RequestModels;
using Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IVnPayService
    {
        string CreateVnpayUrl(HttpContext context, VnpayRequest request);
        VnpayResponse PaymentExecute(IQueryCollection colections);
    }
}

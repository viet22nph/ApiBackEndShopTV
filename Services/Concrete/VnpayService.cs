using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Core.Helpers;
using Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Models.RequestModels;
using Models.ResponseModels;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace Services.Concrete
{
    public class VnpayService : IVnPayService
    {

        private readonly IConfiguration _conf;
        public VnpayService(IConfiguration conf)
        {
            _conf = conf;
        }

        public string CreateVnpayUrl(HttpContext context, VnpayRequest request)
        {
            DateTime expireDate = DateTime.Now.AddMinutes(15); // Ví dụ: 15 phút sau thời gian hiện tại
            var vnpay = new VnPayLibrary();
            string vnp_Returnurl = _conf["Vnpay:vnp_ReturnUrl"];
            string vnp_Url = _conf["Vnpay:vnp_Url"];
            string vnp_TmnCode = _conf["Vnpay:vnp_TmnCode"];
            string vnp_HashSecret = _conf["Vnpay:vnp_HashSecret"];

            vnpay.AddRequestData("vnp_Version", _conf["Vnpay:vnp_Version"]);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (request.Amount * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan hoa don");
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            
            vnpay.AddRequestData("vnp_ExpireDate", expireDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_TxnRef",request.OrderId.ToString());
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }

        public VnpayResponse PaymentExecute(IQueryCollection colections)
        {
            var vnpay = new VnPayLibrary();
            foreach(var (key, value) in colections)
            {
                if(!string.IsNullOrEmpty(key) &&  key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }    
            }
            long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
            long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
            String vnp_SecureHash = colections.FirstOrDefault(x=>x.Key == "vnp_SecureHash").Value;
            string vnp_TxnRef = vnpay.GetResponseData("vnp_TxnRef");
            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _conf["Vnpay:vnp_HashSecret"]);
            if(!checkSignature)
            {
                return new VnpayResponse
                {
                    Success = false,
                };
            }
            return new VnpayResponse
            {
                Success = true,
                PaymentMethod = "VNPAY",
                VnPayResponseCode = vnp_ResponseCode,
                TransactionId = vnpayTranId.ToString(),
                OrderId = Guid.Parse(vnp_TxnRef),
                Token = vnp_SecureHash
            };
        }
    }
}

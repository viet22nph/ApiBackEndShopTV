using AutoMapper;
using Core.Exceptions;
using Core.Interfaces;
using Data.UnitOfWork;
using Models.DTOs.ContactUs;
using Models.DTOs.ContactUs.Request;
using Models.Models;
using Models.ResponseModels;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete
{
    public class ContactUsService : IContactUsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailCoreService _emailService;
        public ContactUsService(IUnitOfWork unitOfWork, IMapper mapper, IEmailCoreService emailCoreService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailCoreService;
        }

        public async Task<BaseResponse<ContactUsDto>> CreateContactUsAsync(ContactUsRequestDto payload)
        {
            var contact = new ContactUs
            {
                FullName = payload.FullName,
                Email = payload.Email,
                Phone = payload.Phone,
                ContactContent = payload.ContactContent,
            };

            // save db
            var contactInsert = await _unitOfWork.Repository<ContactUs>().Insert(contact);
            if (contactInsert == null)
            {
                throw new ApiException($"Internal server error: Insert is failed")
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            var contactMapper = _mapper.Map<ContactUsDto>(contactInsert);
            string body = htmlContact(contact.FullName);
            await _emailService.SendAsync(new Models.DTOs.Email.EmailRequest
            {
                From = "nguyendinh.viet2002np@gmail.com",
                To = contact.Email,
                Subject = "Phản hồi liên hệ của bạn",
                Body = body
            });
            return new BaseResponse<ContactUsDto>(contactMapper, "Create contact us successfully");

        }

        public async Task<(BaseResponse<ICollection<ContactUsDto>>, int)> GetContactUsAsync(int pageSize, int pageNumber)
        {
            var (contactUs, count) = await _unitOfWork.ContactUsRepository.GetContactUsAsync(pageSize, pageNumber);
            if (contactUs == null)
            {
                return (new BaseResponse<ICollection<ContactUsDto>>([], "Contact us"), count);
            }
            var contactDto = _mapper.Map<List<ContactUsDto>>(contactUs);
            return (new BaseResponse<ICollection<ContactUsDto>>(contactDto, "Contact us"), count);
        }

        public async Task<BaseResponse<ContactUsDto>> GetContactUsDetailAsync(Guid id)
        {
            var contact = await _unitOfWork.Repository<ContactUs>().GetById(id);
            if (contact == null)
            {
                new ApiException($"Internal server error: Not found contact us id = {id}")
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            var contactDto = _mapper.Map<ContactUsDto>(contact);
            return new BaseResponse<ContactUsDto>(contactDto, "Contact us");
        }

        public async Task<bool> RemoveContactUsAsync(Guid id)
        {
            var contact = await _unitOfWork.Repository<ContactUs>().GetById(id);
            if (contact == null)
            {
                new ApiException($"Internal server error: Not found contact us id = {id}")
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            var result = await _unitOfWork.Repository<ContactUs>().Delete(contact);
            if (result <= 0)
            {
                return false;
            }
            return true;
        }

        public async Task<BaseResponse<ContactUsDto>> ContactRepliedAsync(Guid id)
        {
            var contact = await _unitOfWork.Repository<ContactUs>().GetById(id);
            if (contact == null)
            {
                new ApiException($"Internal server error: Not found contact us id = {id}")
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            contact.IsReply = true;
            var contactDto = _mapper.Map<ContactUsDto>(contact);
            return new BaseResponse<ContactUsDto>(contactDto, "Contact us");
        }

        public async Task SendMailReply(ReplyContactRequsetDto replyContactRequset)
        {
            string body = htmlReplyContact(replyContactRequset.FullName, replyContactRequset.Body);
            await _emailService.SendAsync(new Models.DTOs.Email.EmailRequest
            {
                From = "nguyendinh.viet2002np@gmail.com",
                To = replyContactRequset.Email,
                Subject = replyContactRequset.Subject,
                Body = body
            });
        }


        #region html contact
        private string htmlContact(string fullName)
        {
            string html = $@"<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Reply to Your Inquiry</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
        }}
        .email-container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}
        .email-header {{
            background-color: #005c29;
            color: #ffffff;
            padding: 10px;
            text-align: center;
            border-top-left-radius: 8px;
            border-top-right-radius: 8px;
        }}
        .email-body {{
            padding: 20px;
            color: #333333;
        }}
        .email-footer {{
            text-align: center;
            padding: 10px;
            background-color:  #005c29;
            color: #ffffff;
            border-bottom-left-radius: 8px;
            border-bottom-right-radius: 8px;
        }}
        .button {{
            display: inline-block;
            padding: 10px 20px;
            margin: 10px 0;
            color: #ffffff;
            background-color: #005c29;
            text-decoration: none;
            border-radius: 5px;
        }}
        .button:hover {{
            background-color: #002244;
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""email-header"">
            <h1>TVfurni shop</h1>
        </div>
        <div class=""email-body"">
           
            <p>Chào anh (chị) {fullName}</p>
            <p>Cảm ơn bạn đã liên hệ với Furni Shop. Chúng tôi đã nhận được yêu cầu của bạn và sẽ phản hồi bạn sớm nhất có thể</p>
            <p>Nếu bạn có bất kỳ câu hỏi nào thêm, vui lòng trả lời email này hoặc liên hệ với chúng tôi qua:</p>
            <p>Email: support@furnishop.com<br>Điện thoại: (123) 456-7890</p>
            <p>Trân trọng,</p>
            <p>Furni Shop Team</p>
            <a href=""https://furnishop.com"" class=""button"">Visit Our Website</a>
        </div>
        <div class=""email-footer"">
            <p>&copy; 2024 Furni Shop. All rights reserved.</p>
        </div>
    </div>
</body>
</html>
";
            return html;
        }
        #endregion

        #region reply
        private string htmlReplyContact(string fullName, string content)
        {
            string html = $@"<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Reply to Your Inquiry</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
        }}
        .email-container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}
        .email-header {{
            background-color: #005c29;
            color: #ffffff;
            padding: 10px;
            text-align: center;
            border-top-left-radius: 8px;
            border-top-right-radius: 8px;
        }}
        .email-body {{
            padding: 20px;
            color: #333333;
        }}
        .email-footer {{
            text-align: center;
            padding: 10px;
            background-color:  #005c29;
            color: #ffffff;
            border-bottom-left-radius: 8px;
            border-bottom-right-radius: 8px;
        }}
        .button {{
            display: inline-block;
            padding: 10px 20px;
            margin: 10px 0;
            color: #ffffff;
            background-color: #005c29;
            text-decoration: none;
            border-radius: 5px;
        }}
        .button:hover {{
            background-color: #002244;
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""email-header"">
            <h1>TVfurni shop</h1>
        </div>
        <div class=""email-body"">
            <p>Chào anh (chị) {fullName}</p>
            <p>Cảm ơn bạn đã liên hệ với TVfurni Shop.</p>
            <p>{content}</p>
            <p>Nếu bạn có bất kỳ câu hỏi nào thêm, vui lòng trả lời email này hoặc liên hệ với chúng tôi qua:</p>
            <p>Email: support@furnishop.com<br>Điện thoại: (123) 456-7890</p>
            <p>Trân trọng,</p>
            <p>Furni Shop Team</p>
            <a href=""https://furnishop.com"" class=""button"">Visit Our Website</a>
        </div>
        <div class=""email-footer"">
            <p>&copy; 2024 Furni Shop. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
            return html;
        }
        #endregion
    }
}

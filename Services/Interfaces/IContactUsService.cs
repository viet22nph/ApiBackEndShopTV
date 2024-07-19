using Models.DTOs.ContactUs;
using Models.DTOs.ContactUs.Request;
using Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IContactUsService
    {
        Task<BaseResponse<ContactUsDto>> CreateContactUsAsync(ContactUsRequestDto payload);
        Task<(BaseResponse<ICollection<ContactUsDto>>, int)> GetContactUsAsync(int pageSize, int pageNumber);
        Task<BaseResponse<ContactUsDto>> GetContactUsDetailAsync(Guid id);
        Task<bool> RemoveContactUsAsync(Guid id);
        Task<BaseResponse<ContactUsDto>> ContactRepliedAsync(Guid id);
        Task SendMailReply(ReplyContactRequsetDto replyContactRequset);

    }
}

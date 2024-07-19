using Caching;
using Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.ContactUs.Request;
using Services.Interfaces;
using System.Net;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {

        private readonly IContactUsService _contactUs;
        private readonly ICacheManager _cacheManager;

        public ContactUsController(IContactUsService contactUsService, ICacheManager cacheManager)
        {
            _contactUs = contactUsService;
            _cacheManager = cacheManager;
        }

        [HttpPost("create-contact-us")]
        public async Task<IActionResult> CreateContactUs(ContactUsRequestDto payload)
        {
            var result = await _contactUs.CreateContactUsAsync(payload);

            return Ok(result);
        }
        [HttpGet()]
        public async Task<IActionResult> GetContactUs(int pageNumber = 1, int pageSize = 20)
        {
            var (result, count) = await _contactUs.GetContactUsAsync(pageSize, pageNumber);
            return Ok(new { 
                message = result.Message,
                data = result.Data, 
                pageNumber,
                pageSize,
                count
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactUsDetail(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ApiException($"Internal server error: Id is empty")
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            var result = await _contactUs.GetContactUsDetailAsync(id);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveContact(Guid id)
        {
            if (id == Guid.Empty)
            {

                throw new ApiException($"Internal server error: Id is empty")
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            var result = await _contactUs.RemoveContactUsAsync(id);
            if(!result)
            {
                throw new ApiException($"Internal server error: Remove contact is failed")
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            return NoContent();
        }
        [HttpPut("contact-replied/{id}")]
        public async Task<IActionResult> ContactRepliedAsync(Guid id)
        {
            if (id == Guid.Empty)
            {

                throw new ApiException($"Internal server error: Id is empty")
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            var result = await _contactUs.ContactRepliedAsync(id);
           
            return Ok(result);
        }
        [HttpPost("reply")]
        public async Task<IActionResult> Reply(ReplyContactRequsetDto paylod)
        {
            await _contactUs.SendMailReply(paylod);
            return NoContent();
        }
        

    }
}

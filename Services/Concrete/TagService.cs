using AutoMapper;
using Core.Exceptions;
using Data.UnitOfWork;
using Models.DTOs;
using Models.DTOs.Tag;
using Models.DTOs.Tag.request;
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
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TagService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<TagDto>> CreateTagAsync(TagRequsetDto payload)
        {
            var checkExists = await _unitOfWork.TagRepository.CheckTagTitleExistsAsync(payload.TagTitle);
            if (checkExists)
            {
                throw new ApiException($"Tag title {payload.TagTitle} is exists")
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            var tag = new Tag
            {
                TagTitle = payload.TagTitle,
            };

            var tagInsert = await _unitOfWork.Repository<Tag>().Insert(tag);
            if (tagInsert == null)
            {
                throw new ApiException($"Tag title insert failed")
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            var tagDto = _mapper.Map<TagDto>(tagInsert);
            return new BaseResponse<TagDto>(tagDto, "Insert tag successfully");
        }

        public async Task<BaseResponse<ICollection<TagDto>>> GetTagAsync()
        {
            var tags = await _unitOfWork.Repository<Tag>().GetAll();
            if (tags == null)
                return new BaseResponse<ICollection<TagDto>>([], "Tags");

            var tagsDto = _mapper.Map<List<TagDto>>(tags);
           return new BaseResponse<ICollection<TagDto>>(tagsDto, "Tags");
        }

    }
}

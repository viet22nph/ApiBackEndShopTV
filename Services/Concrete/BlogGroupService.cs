using AutoMapper;
using Core.Exceptions;
using Data.Contexts;
using Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.BlogGroup;
using Models.DTOs.BlogGroup.Request;
using Models.Models;
using Models.ResponseModels;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete
{
    public class BlogGroupService: IBlogGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        public BlogGroupService(IUnitOfWork unitOfWork, IMapper mapper, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
        }
        
        public async Task<BaseResponse<BlogGroupDto>> CreateBlogGroupAsync(BlogGroupRequestDto payload)
        {
                var checkExists = await _unitOfWork.BlogGroupRepository.CheckNameExistsAsync(payload.Name);
                if (checkExists)
                {
                    throw new ApiException($"Blog group '{payload.Name}' is exists!")
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                var blogGroup = new BlogGroup
                {
                    Name = payload.Name,
                    Description = payload.Description
                };
                var blogGroupInsert = await _unitOfWork.Repository<BlogGroup>().Insert(blogGroup);
                if (blogGroupInsert == null)
                {
                    throw new ApiException($"Internal server error: Insert blog group {payload.Name} is failed")
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                var blogGroupDto = _mapper.Map<BlogGroupDto>(blogGroupInsert);
                return new BaseResponse<BlogGroupDto>(blogGroupDto, $"Insert blog group {payload.Name} is success");

        }


        public async Task<BaseResponse<ICollection<BlogGroupDto>>> GetBlogGroupAsync()
        {
            var blogGroups = await _unitOfWork.Repository<BlogGroup>().GetAll();
            if(blogGroups == null)
            {
                return new BaseResponse<ICollection<BlogGroupDto>>([], "Blog groups");
            }

            var blogGroupsDto = _mapper.Map<List<BlogGroupDto>>(blogGroups);
                return new BaseResponse<ICollection<BlogGroupDto>>(blogGroupsDto, "Blog groups");
        }

        public async Task<BaseResponse<BlogGroupDto>> UpdateBlogGroupAsync(UpdateBlogGroupRequestDto payload)
        {
           

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var blogGroup = await _unitOfWork.Repository<BlogGroup>().GetById(payload.Id);
                // Check if blog group exists
                if (blogGroup == null)
                {
                    throw new ApiException($"Blog group not found with id = {payload.Id}")
                    {
                        StatusCode = (int)HttpStatusCode.NotFound
                    };
                }

                blogGroup.Name = payload.Name;
                blogGroup.Description = payload.Description;

                await _unitOfWork.Repository<BlogGroup>().Update(blogGroup);

                var blogGroupDto = _mapper.Map<BlogGroupDto>(blogGroup);
   
                await transaction.CommitAsync();

                return new BaseResponse<BlogGroupDto>
                {
                    Data = blogGroupDto,
                    Message = "Blog group updated successfully"

                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException($"Internal server error: {ex.Message}")
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<bool> RemoveBlogGroupAsync(Guid blogGroupId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var blogGroup = await _unitOfWork.Repository<BlogGroup>().GetById(blogGroupId);
                if (blogGroup == null)
                {
                    throw new ApiException($"Blog group not found with id = {blogGroupId}")
                    {
                        StatusCode = (int)HttpStatusCode.NotFound
                    };
                }

                var blogs = await _unitOfWork.Repository<Blog>().FindAll(b => b.BlogGroupId == blogGroupId);
                if (blogs.Any())
                {
                    foreach (var blog in blogs)
                    {
                        var tagsBlogs = await _unitOfWork.Repository<TagBlog>().FindAll(tb => tb.BlogId == blog.Id);
                        foreach (var tagBlog in tagsBlogs)
                        {
                            await _unitOfWork.Repository<TagBlog>().Delete(tagBlog);
                        }

                        await _unitOfWork.Repository<Blog>().Delete(blog);
                    }
                }
                await _unitOfWork.Repository<BlogGroup>().Delete(blogGroup);
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException($"Internal server error: {ex.Message}")
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
        public async Task<BaseResponse<BlogGroupDetailDto>> GetBlogGroupDetailAsync(Guid blogGroupId)
        {
            try
            {
                var blogGroup = await _unitOfWork.BlogGroupRepository.GetBlogGroupDetailAsync(blogGroupId);

                if (blogGroup == null)
                {
                    throw new ApiException($"Blog group not found with id = {blogGroupId}")
                    {
                        StatusCode = (int)HttpStatusCode.NotFound
                    };
                }
                // Map the blog group to BlogGroupDetailDto
                var blogGroupDetailDto = _mapper.Map<BlogGroupDetailDto>(blogGroup);

                return new BaseResponse<BlogGroupDetailDto>
                {
                    Data = blogGroupDetailDto,
                    Message = "Blog group details retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}")
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}

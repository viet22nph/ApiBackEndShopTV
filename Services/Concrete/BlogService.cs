using AutoMapper;
using Core.Exceptions;
using Data.Contexts;
using Data.UnitOfWork;
using Microsoft.AspNetCore.Http.HttpResults;
using Models.DTOs.Blog;
using Models.DTOs.Blog.Request;
using Models.DTOs.Cart;
using Models.Models;
using Models.ResponseModels;
using Services.Interfaces;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete
{
    public class BlogService: IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        public BlogService(IUnitOfWork unitOfWork, IMapper mapper, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
        }

        public async Task<BaseResponse<BlogDto>> CreateBlogAsync(BlogRequestDto payload)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // check blog group
                var blogGroup = await _unitOfWork.Repository<BlogGroup>().GetById(payload.BlogGroupId);
                if (blogGroup == null)
                {
                    throw new ApiException($"Not found blog group id = {payload.BlogGroupId}")
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }
                // check auth
                var auth = await _unitOfWork.UserRepository.GetUserById(payload.AuthorId);
                if (auth == null)
                {
                    throw new ApiException($"Not found author id = {payload.BlogGroupId}")
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                var newBlog = new Blog
                {
                    BlogImage = payload.BlogImage,
                    Title = payload.Title,
                    Content = payload.Content,
                    AuthorId = payload.AuthorId,
                    BlogGroupId = payload.BlogGroupId,
                    BlogGroup = blogGroup,
                    Author = auth,
                    TagBlogs = new List<TagBlog>()
                };

                // check tag if tag not exsits -> insert
                if (payload.TagsBlog != null &&  payload.TagsBlog.Count > 0)
                {
                    foreach (var tagItem in payload.TagsBlog)
                    {

                        var tag = await _unitOfWork.Repository<Tag>().Find(x => x.TagTitle == tagItem);
                        if (tag == null)
                        {

                            tag = await _unitOfWork.Repository<Tag>().Insert(new Tag { TagTitle = tagItem });
                            if (tag == null)
                            {
                                // rollback

                                await transaction.RollbackAsync();
                                throw new ApiException($"Error insert")
                                {
                                    StatusCode = (int)HttpStatusCode.BadRequest
                                };
                            }
                        }

                        newBlog.TagBlogs.Add(new TagBlog {Tag = tag, TagId =tag.Id, Blog = newBlog, BlogId = newBlog.Id });
                    }
                }
                var blogInsert = await _unitOfWork.Repository<Blog>().Insert(newBlog);
                if (blogInsert == null)
                {

                    await transaction.RollbackAsync();
                    throw new ApiException("Error inserting blog")
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }
                var blogDto = _mapper.Map<BlogDto>(blogInsert);
                await transaction.CommitAsync();
                return new BaseResponse<BlogDto>
                {
                    Data = blogDto,
                    Message = "Blog created successfully"
                };
            }
            catch (Exception ex)
            {
                 await transaction.RollbackAsync();
                throw new ApiException($"Internal server error: {ex}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }


        public async Task<(BaseResponse<ICollection<BlogDto>>, int)> GetBlogAsync(int pageNumber, int pageSize)
        {
            var (blogs, count) = await _unitOfWork.BlogRepository.GetBlogsAsync(pageNumber, pageSize);
            if (count == 0)
            {
                return (new BaseResponse<ICollection<BlogDto>>([], "Blogs"), count);
            }
            var blogsDto = _mapper.Map<List<BlogDto>>(blogs);
            return (new BaseResponse<ICollection<BlogDto>>(blogsDto, "Blogs"), count);
        }

        public async Task<bool> RemoveBlogAsync(Guid id)
        {
            // check id
            var blog = await _unitOfWork.Repository<Blog>().GetById(id);
            if(blog == null)
            {
                throw new ApiException($"Not found blog id = {id}")
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            int rm = await _unitOfWork.Repository<Blog>().Delete(blog);
            if (rm == 0)
            {
                return false;
            }
            return true;
        }
        
        public async Task<(BaseResponse<ICollection<BlogDto>>, int)> GetBlogByGroupIdAsync(Guid id, int pageNumber, int pageSize)
        {
            var (blogs, count) = await _unitOfWork.BlogRepository.GetBlogByGroupIdAsync(id,pageNumber, pageSize);
            if (count == 0)
            {
                return (new BaseResponse<ICollection<BlogDto>>([], "Blogs"), count);
            }
            var blogsDto = _mapper.Map<List<BlogDto>>(blogs);
            return (new BaseResponse<ICollection<BlogDto>>(blogsDto, "Blogs"), count);
        }
        public async Task<(BaseResponse<ICollection<BlogDto>>, int)> GetBlogByTagIdAsync(Guid id, int pageNumber, int pageSize)
        {
            var (blogs, count) = await _unitOfWork.BlogRepository.GetBlogsByTagIdAsync(id, pageNumber, pageSize);
            if (count == 0)
            {
                return (new BaseResponse<ICollection<BlogDto>>([], "Blogs"), count);
            }
            var blogsDto = _mapper.Map<List<BlogDto>>(blogs);
            return (new BaseResponse<ICollection<BlogDto>>(blogsDto, "Blogs"), count);
        }
        public async Task<BaseResponse<BlogDto>> GetBlogByIdAsync(Guid id)
        {
            

            var blog = await _unitOfWork.BlogRepository.GetBlogByIdAsync(id);
            if(blog == null)
            {
                throw new ApiException($"Blog not found with id = {id}")
                {
                    StatusCode = (int)HttpStatusCode.NotFound
                };

            }
            var blogDto = _mapper.Map<BlogDto>(blog);

            return new BaseResponse<BlogDto>
            {
                Data = blogDto,
                Message = "Blog updated successfully"
            };
        }

        public async Task<BaseResponse<BlogDto>> UpdateBlogAsync(UpdateBlogRequestDto payload)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Check if blog exists
                var blog = await _unitOfWork.BlogRepository.GetBlogByIdAsync(payload.Id);
                if (blog == null)
                {
                    throw new ApiException($"Blog not found with id = {payload.Id}")
                    {
                        StatusCode = (int)HttpStatusCode.NotFound
                    };
                }

                // Check blog group
                var blogGroup = await _unitOfWork.Repository<BlogGroup>().GetById(payload.BlogGroupId);
                if (blogGroup == null)
                {
                    throw new ApiException($"Blog group not found with id = {payload.BlogGroupId}")
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                // Check author
                var auth = await _unitOfWork.UserRepository.GetUserById(payload.AuthorId);
                if (auth == null)
                {
                    throw new ApiException($"Author not found with id = {payload.AuthorId}")
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                // Update blog properties
                blog.BlogImage = payload.BlogImage;
                blog.Title = payload.Title;
                blog.Content = payload.Content;
                blog.AuthorId = payload.AuthorId;
                blog.BlogGroupId = payload.BlogGroupId;
                blog.BlogGroup = blogGroup;
                blog.Author = auth;

                // Update tags
                var existingTags = blog.TagBlogs.ToList();
                blog.TagBlogs.Clear();

                if (payload.TagBlogs.Count > 0)
                {
                    foreach (var tagItem in payload.TagBlogs)
                    {
                        var tag = await _unitOfWork.Repository<Tag>().Find(x => x.TagTitle == tagItem);
                        if (tag == null)
                        {
                            tag = new Tag { TagTitle = tagItem };
                            await _unitOfWork.Repository<Tag>().Insert(tag);
                        }

                        blog.TagBlogs.Add(new TagBlog { TagId = tag.Id, Blog = blog });
                    }
                }

                // Update blog in repository
                await _unitOfWork.Repository<Blog>().Update(blog);
                // Map to DTO
                var blogDto = _mapper.Map<BlogDto>(blog);

                // Commit transaction
                await transaction.CommitAsync();

              

                return new BaseResponse<BlogDto>
                {
                    Data = blogDto,
                    Message = "Blog updated successfully"
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
    }
}

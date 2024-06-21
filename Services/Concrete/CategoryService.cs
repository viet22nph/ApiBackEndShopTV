using Application.DAL.Models;
using AutoMapper;
using Core.Exceptions;
using Data.Repos;
using Data.UnitOfWork;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using Models.DTOs.Category;
using Models.ResponseModels;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace Services.Concrete
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly ICategoryRepository _categoryRepo;
        private readonly IGenericRepository<Category> _repository;
        private readonly IMapper _mapper;
        public CategoryService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IGenericRepository<Category>  repository
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repository = repository;
        }

       

        #region Get Categories 
        public async Task<BaseResponse<List<CategoryDto>>> GetCategories()
        {
            try
            {
                var categories = await _repository.GetAll();
                if(categories.IsNullOrEmpty())
                    throw new ApiException("Not found categories") { StatusCode = (int)HttpStatusCode.NotFound };
                var categoriesDto = _mapper.Map<List<Category>, List<CategoryDto>>(categories);
                return new BaseResponse<List<CategoryDto>>(categoriesDto, "Categories");
                
            }catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };

            }
        }

        #endregion

        #region GetCategory
        public async Task<BaseResponse<CategoryDto>> GetCategory(Guid id)
        {
            try
            {
                var category = await _repository.GetById(id);
                if (category == null)
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                var categoryDto = _mapper.Map<CategoryDto>(category);
                return new BaseResponse<CategoryDto>(categoryDto, "Category");

            }
            catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };

            }
        }
        #endregion

        #region Insert Category
        public async Task<BaseResponse<CategoryDto>> InsertCategory(CategoryRequest request)
        {

            try
            {
                var cateory = await _repository.Find(x => x.Name == request.Name);
                if (cateory != null)
                {
                    throw new ApiException($"You are not insert category with '{request.Name}'.") { StatusCode = (int)HttpStatusCode.BadRequest };
                }    

                var mapCategory = _mapper.Map<Category>(request);
                mapCategory.Id = Guid.NewGuid();
                mapCategory.NomalizedName = request.Name.ToUpper();
                var insert =  await _repository.Insert(mapCategory);
                if (insert == null)
                    throw new ApiException("Insert category faill") { StatusCode = (int)HttpStatusCode.BadRequest };
                var categoryDto = _mapper.Map<CategoryDto>(insert);
                return new BaseResponse<CategoryDto>(categoryDto, "Category");
            }
            catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };

            }
        }
        #endregion

        #region UpdateCategory
        public async Task<BaseResponse<CategoryDto>> UpdateCategory(Guid id,CategoryUpdateRequest request)
        {
            try
            {
                var category = await _repository.GetById(id);
                if (category == null)
                {
                    throw new ApiException($"Not found") { StatusCode = (int)HttpStatusCode.NotFound};
                }
                EntityUpdater.UpdateIfNotNull(request.Name, value => category.Name = value);
                EntityUpdater.UpdateIfNotNull(request.Description, value => category.Description = value);
                category.CategoryParent = request.CategoryParent;
                category.NomalizedName = request.Name.ToUpper();
                var cateoryUpdate = await _repository.Update(category);
                var res = _mapper.Map<CategoryDto>(cateoryUpdate);
                return new BaseResponse<CategoryDto>(res, "Category");
            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };

            }
        }
        #endregion

        #region Delete Category

        public async Task<BaseResponse<string>> DeleteCategory(Guid id)
        {
            try
            {
                var category = await _repository.GetById(id);
                if (id == null)
                {
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }
                var categoryProduct = await _unitOfWork.Repository<Product>().FindAll(x => x.CategoryId == category.Id);
                if (categoryProduct != null && categoryProduct?.Count > 0)
                {
                    throw new ApiException("Not Remove") { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                var change = await _repository.Delete(category);
                if (change < 1)
                {
                    throw new ApiException("Not Remove") { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                return new BaseResponse<string>("Remove success");

            }
            catch (Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };

            }


        }
        #endregion

        public async Task<BaseResponse<ICollection< CategoryDto>>> GetCategoriesParent()
        {
            try
            {
                var data = await _unitOfWork.CategoryRepository.GetCategoriesParent();
                var res = _mapper.Map<List<CategoryDto>>(data);
                return new BaseResponse<ICollection<CategoryDto>>(res, "Categories Parent");

            }catch(Exception  ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };

            }


        }
    }
}

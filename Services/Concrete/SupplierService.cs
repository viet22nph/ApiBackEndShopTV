using Application.DAL.Models;
using AutoMapper;
using Core.Exceptions;
using Data.Repos;
using Data.UnitOfWork;
using Microsoft.IdentityModel.Tokens;
using Models.DTOs.Category;
using Models.DTOs.Supplier;
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
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly ICategoryRepository _categoryRepo;
        private readonly IGenericRepository<Supplier> _repository;
        private readonly IMapper _mapper;
        public SupplierService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IGenericRepository<Supplier> repository
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<BaseResponse<string>> DeleteSupplier(Guid id)
        {
            try
            {
                var supplier = await _repository.GetById(id);
                if (supplier == null)
                {
                    throw new ApiException("Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }
                var supProduct = await _unitOfWork.Repository<Product>().FindAll(x => x.SupplierId == supplier.Id);
                if (supProduct != null && supProduct.Count > 0)
                {
                    throw new ApiException("Remove fail") { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                var check = await _repository.Delete(supplier);
                if (check < 0)
                {
                    throw new ApiException("Remove fail") { StatusCode = (int)HttpStatusCode.BadRequest };

                }
                return new BaseResponse<string>("Remmove success");
            }
            catch(Exception ex) {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            
        }

        public async Task<BaseResponse<SupplierDto>> GetSupplier(Guid id)
        {
            try
            {
                var supplier = await _repository.GetById(id);
                if (supplier == null)
                    throw new ApiException("Not found supplier") { StatusCode = (int)HttpStatusCode.NotFound };
                var supplierDto = _mapper.Map< SupplierDto>(supplier);
                return new BaseResponse<SupplierDto>(supplierDto, "Supplier");

            }
            catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };

            }
        }

        public async Task<BaseResponse<List<SupplierDto>>> GetSuppliers()
        {  try
            {
                var suppliers = await _repository.GetAll();
                if(suppliers.IsNullOrEmpty())
                    throw new ApiException("Not found Suppliers") { StatusCode = (int)HttpStatusCode.NotFound };
                var suppliersDto = _mapper.Map<List<Supplier>, List<SupplierDto>>(suppliers);
                return new BaseResponse<List<SupplierDto>>(suppliersDto, "Suppliers");
                
            }catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };

            }
        }

        public async Task<BaseResponse<SupplierDto>> InsertSupplier(SupplierRequest request)
        {
            try
            {
                var supplier = await _repository.Find(x => x.SupplierName == request.SupplierName);
                if (supplier != null)
                {
                    throw new ApiException($"You are not insert supplier with '{request.SupplierName}'.") { StatusCode = (int)HttpStatusCode.BadRequest };
                }

                var mapSupplier = _mapper.Map<Supplier>(request);
                mapSupplier.Id = Guid.NewGuid();
                var insert = await _repository.Insert(mapSupplier);
                if (insert == null)
                    throw new ApiException("Insert category faill") { StatusCode = (int)HttpStatusCode.BadRequest };
                var supplierDto = _mapper.Map<SupplierDto>(insert);
                return new BaseResponse<SupplierDto>(supplierDto, "Supplier");
            }
            catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };

            }
        }

        public async Task<BaseResponse<SupplierDto>> UpdateSupplier(Guid id, SupplierRequest request)
        {

            try
            {
                var supplier = await _repository.GetById(id);
                if (supplier == null)
                {
                    throw new ApiException($"Not found") { StatusCode = (int)HttpStatusCode.NotFound };
                }
                supplier.SupplierName = request.SupplierName;
                supplier.Address = request.Address;
                supplier.ContactPhone = request.ContactPhone;
                supplier.ContactPerson = request.ContactPerson;
                supplier.Notes = request.Notes;
               var supplierUpdate = await _repository.Update(supplier);
                var res = _mapper.Map<SupplierDto>(supplierUpdate);

                return new BaseResponse<SupplierDto>(res, "Supplier");
            }
            catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };

            }
        }
    }
}

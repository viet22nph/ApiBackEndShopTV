using Models.DTOs.Category;
using Models.DTOs.Supplier;
using Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ISupplierService
    {

        Task<BaseResponse<List<SupplierDto>>> GetSuppliers();

        Task<BaseResponse<SupplierDto>> GetSupplier(Guid id);

        Task<BaseResponse<SupplierDto>> InsertSupplier(SupplierRequest request);
        Task<BaseResponse<SupplierDto>> UpdateSupplier(Guid id,SupplierRequest request);
        Task<BaseResponse<string>> DeleteSupplier(Guid id);
    }
}

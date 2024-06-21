using Models.DTOs.Discount;
using Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IDiscountService
    {
        Task<BaseResponse<DiscountDto>> CreateDiscount(DiscountRequest request);
        Task UpdateAllDiscountStatus();
        Task<BaseResponse<DiscountDto>> PauseDiscountStatus(Guid id);
        Task<BaseResponse<DiscountDto>> CancelledDiscountStatus(Guid id);

        Task<BaseResponse<DiscountDto>> GetDiscount(Guid id);
        Task<BaseResponse<ICollection<DiscountDto>>> GetDiscounts(int pageNumber =0, int pageSize =10);

        Task<BaseResponse<DiscountDto>> UpdateDateTime(Guid id,DiscountDateTimeRequest request);
        Task<BaseResponse<DiscountDto>> ContinueDiscountStatus(Guid id);

    }
}

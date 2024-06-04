using Application.DAL.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.DiscountRepo
{
    public interface IDiscountRepository
    {
        Task<int> CountAsync();
        Task<ICollection<Discount>> GetDiscounts(int pageNumber, int pageSize);
        Task<ICollection<ProductDiscount>> GetDiscountsByProductId(Guid id);
    }
}

using Application.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.OrderRepo
{
    public interface IOrderRepository
    {

        Task<ICollection<Order>> GetOrders(int pageNumber, int pageSize);
        Task<Order> GetOrderDetail(Guid id);

    }
}

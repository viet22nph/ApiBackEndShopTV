using Data.Repos;
using Data.Repos.DiscountRepo;
using Data.Repos.OrderRepo;
using Data.Repos.ProductRepo;
using Data.Repos.ReviewRepo;
using Data.Repos.UserRepo;
using Models.DbEntities;
using System;
using System.Threading.Tasks;

namespace Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
        IUserRepository UserRepository { get; }
        IProductRepository ProductRepository { get; }
        IOrderRepository OrderRepository { get; }
        IDiscountRepository DiscountRepository { get; }
        IReviewRepository ReviewRepository { get; }
        Task<int> Complete();
        bool HasChanges();
    }
}

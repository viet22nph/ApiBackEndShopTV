using Data.Repos;
using Data.Repos.BannerRepo;
using Data.Repos.BlogGroupRepo;
using Data.Repos.BlogRepo;
using Data.Repos.CategoryRepo;
using Data.Repos.ContactUs;
using Data.Repos.DiscountRepo;
using Data.Repos.GroupBannerRepo;
using Data.Repos.OrderRepo;
using Data.Repos.ProductRepo;
using Data.Repos.ReviewRepo;
using Data.Repos.TagRepo;
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
        ICategoryRepository CategoryRepository { get; }
        IGroupBannerRepository GroupBannerRepository { get; }
        IBannerRepository BannerRepository { get; }
        IContactUsRepository ContactUsRepository { get; }
        ITagRepository TagRepository { get; }
        IBlogGroupRepository BlogGroupRepository { get; }
        IBlogRepository BlogRepository { get; }
        Task<int> Complete();
        bool HasChanges();
    }
}

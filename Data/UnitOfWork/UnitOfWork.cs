using Data.Contexts;
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
using System.Collections;
using System.Threading.Tasks;

namespace Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private Hashtable _repositories;
        
        public IUserRepository UserRepository => new UserRepository(_context);

        public IProductRepository ProductRepository => new ProductRepository(_context);

        public IOrderRepository OrderRepository =>  new OrderRepository(_context);

        public IDiscountRepository DiscountRepository =>  new DiscountRepository(_context);

        public IReviewRepository ReviewRepository =>  new ReviewRepository(_context);

        public ICategoryRepository CategoryRepository =>  new CategoryRepository(_context);

        public IGroupBannerRepository GroupBannerRepository => new GroupBannerRepository(_context);
        public IBannerRepository BannerRepository => new BannerRepository(_context);

        public IContactUsRepository ContactUsRepository =>  new ContactUsRepository(_context);

        public IBlogGroupRepository BlogGroupRepository => new BlogGroupRepository(_context);
        public ITagRepository TagRepository => new TagRepository(_context);

        public IBlogRepository BlogRepository => new BlogRepository(_context);

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }
        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories == null) _repositories = new Hashtable();

            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);

                _repositories.Add(type, repositoryInstance);
            }

            return (IGenericRepository<TEntity>)_repositories[type];
        }
    }
}

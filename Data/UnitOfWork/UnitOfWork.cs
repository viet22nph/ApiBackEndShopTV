using Data.Contexts;
using Data.Repos;
using Data.Repos.DiscountRepo;
using Data.Repos.OrderRepo;
using Data.Repos.ProductRepo;
using Data.Repos.ReviewRepo;
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

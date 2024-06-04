using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Repos
{
    public interface IGenericRepository<T> where T : class
    {
         Task<List<T>> GetAll();
        Task<T> GetById(Guid id);
        Task<T> Find(Expression<Func<T, bool>> match);
        Task<List<T>> FindAll(Expression<Func<T, bool>> match);
        Task<List<T>> FindAllAsync(Expression<Func<T, bool>> match);
        Task<T> Insert(T entity);
        Task<bool> BulkInsert(List<T> entities);
        Task<T> Update(T entity);
        Task<int> Delete(T entity);
    }
}

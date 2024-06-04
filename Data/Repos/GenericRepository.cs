using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Repos
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        public GenericRepository(ApplicationDbContext context = null)
        {
            _context = context;
        }

        public async Task<bool> BulkInsert(List<T> entities)
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            try
            {
                _context.Set<T>().AddRange(entities);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            return  await _context.SaveChangesAsync();
        }

        public async Task<T> Find(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(match);
        }

        public async Task<List<T>> FindAll(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().Where(match).ToListAsync();
        }

        public async Task<List<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().Where(match).ToListAsync();
        }

        public async Task<List<T>> GetAll()
        {
            return  await _context.Set<T>().ToListAsync();
        }

        public  async Task<T> GetById(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> Insert(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            _context.SaveChanges();
            return entity;
        }

        public async Task<T> Update(T entity)
        {
            if (entity == null)
                return null;
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}

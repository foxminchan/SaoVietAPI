using System.Linq.Expressions;
using Domain.Interfaces;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public abstract class GenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly string _cacheKey = $"{typeof(T)}";
        private readonly ICache _memoryCache;
        private readonly SemaphoreSlim _lock = new(1, 1);

        protected GenericRepository(ApplicationDbContext context, ICache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
            _dbSet = _context.Set<T>();
        }

        public virtual IEnumerable<T> GetAll()
        {
            if (_memoryCache.TryGet(_cacheKey, out IEnumerable<T> entities)) return entities;
            _lock.Wait();
            try
            {
                if (_memoryCache.TryGet(_cacheKey, out entities)) return entities;
                entities = _dbSet.AsNoTracking().ToList();
                _memoryCache.Set(_cacheKey, entities);
                return entities;
            }
            finally
            {
                _lock.Release();
            }
        }

        public virtual void Insert(T entity)
        {
            try
            {
                _dbSet.Add(entity);
                BackgroundJob.Enqueue(() => RefreshCache());
            }
            catch (DbUpdateException e)
            {
                throw new DbUpdateException("Error while inserting entity into the database.", e);
            }
        }

        public virtual void Update(T entity)
        {
            try
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
                BackgroundJob.Enqueue(() => RefreshCache());
            }
            catch (DbUpdateException e)
            {
                throw new DbUpdateException("Error while updating entity in the database.", e);
            }
        }

        public virtual void Delete(T entity)
        {
            try
            {
                if (_context.Entry(entity).State == EntityState.Detached)
                    _dbSet.Attach(entity);
                _dbSet.Remove(entity);
                BackgroundJob.Enqueue(() => RefreshCache());
            }
            catch (DbUpdateException e)
            {
                throw new DbUpdateException("Error while deleting entity from the database.", e);
            }
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            try
            {
                var objects = _dbSet.Where(where).AsEnumerable();
                foreach (var obj in objects)
                    _dbSet.Remove(obj);
                BackgroundJob.Enqueue(() => RefreshCache());
            }
            catch (DbUpdateException e)
            {
                throw new DbUpdateException("Error while deleting entity from the database.", e);
            }
        }

        public virtual int Count(Expression<Func<T, bool>> where) => _dbSet.Count(where);

        public virtual T? GetById(object? id) => _dbSet.Find(id);

        public virtual IEnumerable<T> GetList(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "",
            int skip = 0,
            int take = 0)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
                query = query.Where(filter);

            query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (orderBy != null)
                query = orderBy(query);

            if (skip != 0)
                _ = query.Skip(skip);

            if (take != 0)
                _ = query.Take(take);

            return query.ToList();
        }

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where) => _dbSet.Where(where).ToList();

        public virtual bool Any(Expression<Func<T, bool>> where) => _dbSet.Any(where);

        // ReSharper disable once MemberCanBePrivate.Global
        public async Task RefreshCache()
        {
            _memoryCache.Remove(_cacheKey);
            var entities = await _dbSet.AsNoTracking().ToListAsync();
            _memoryCache.Set(_cacheKey, entities);
        }
    }
}

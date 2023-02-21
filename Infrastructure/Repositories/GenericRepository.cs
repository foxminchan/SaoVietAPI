using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
        private readonly ICache _redisCache;
        private readonly SemaphoreSlim _lock = new(1, 1);

        protected GenericRepository(ApplicationDbContext context, ICache cache)
        {
            _context = context;
            _redisCache = cache;
            _dbSet = _context.Set<T>();
        }

        public virtual IEnumerable<T> GetAll()
        {
            if (_redisCache.TryGet(_cacheKey, out IEnumerable<T> entities))
                return entities;
            try
            {
                _lock.Wait();
                if (_redisCache.TryGet(_cacheKey, out entities))
                    return entities;
                entities = _dbSet.AsNoTracking().ToList();
                _redisCache.Set(_cacheKey, entities);
                _redisCache.Subscribe(cacheKey =>
                {
                    if (cacheKey != _cacheKey)
                        return;
                    entities = _dbSet.AsNoTracking().ToList();
                    _redisCache.Set(_cacheKey, entities);
                });
                return entities;
            }
            finally
            {
                _lock.Release();
            }
        }

        public virtual void Insert(T entity)
        {
            _dbSet.Add(entity);
            _redisCache.Remove(_cacheKey);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _redisCache.Remove(_cacheKey);
        }

        public virtual void Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
                _dbSet.Attach(entity);
            _dbSet.Remove(entity);
            _redisCache.Remove(_cacheKey);
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            var objects = _dbSet.Where(where).AsEnumerable();
            foreach (var obj in objects)
                _dbSet.Remove(obj);
            _redisCache.Remove(_cacheKey);
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

            return query;
        }

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where) => _dbSet.Where(where);

        public virtual bool Any(Expression<Func<T, bool>> where) => _dbSet.Any(where);
    }
}

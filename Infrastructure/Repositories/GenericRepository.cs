using System.Linq.Expressions;
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

        protected GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual IEnumerable<T> GetAll() => _dbSet.ToList();

        public virtual void Insert(T entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Update(T entity, Expression<Func<T, bool>> where)
        {
            var obj = _dbSet.Where(where).FirstOrDefault() ?? throw new InvalidOperationException();
            _context.Entry(obj).CurrentValues.SetValues(entity);
        }

        public virtual void Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
                _dbSet.Attach(entity);
            _dbSet.Remove(entity);
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            var objects = _dbSet.Where(where).AsEnumerable();
            foreach (T obj in objects)
                _dbSet.Remove(obj);
        }

        public virtual int Count(Expression<Func<T, bool>> where) => _dbSet.Count(where);

        public virtual T GetById(object? id) => _dbSet.Find(id) ?? throw new InvalidOperationException();

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

    }
}

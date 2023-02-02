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

        public virtual IEnumerable<T> GetAll() => _dbSet.AsNoTracking().ToList();

        public virtual void Insert(T entity)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _dbSet.Add(entity);
                transaction.Commit();
            }
            catch (DbUpdateException e)
            {
                transaction.Rollback();
                throw new Exception("Error while inserting entity into the database.", e);
            }
        }

        public virtual void Update(T entity)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
                transaction.Commit();
            }
            catch (DbUpdateException e)
            {
                transaction.Rollback();
                throw new Exception("Error while updating entity in the database.", e);
            }
        }

        public virtual void Update(T entity, Expression<Func<T, bool>> where)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var obj = _dbSet.FirstOrDefault(where);
                if (obj == null) return;
                _context.Entry(obj).CurrentValues.SetValues(entity);
                transaction.Commit();
            }
            catch (DbUpdateException e)
            {
                transaction.Rollback();
                throw new Exception("Error while updating entity in the database.", e);
            }
        }

        public virtual void Delete(T entity)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (_context.Entry(entity).State == EntityState.Detached)
                    _dbSet.Attach(entity);
                _dbSet.Remove(entity);
                transaction.Commit();
            }
            catch (DbUpdateException e)
            {
                transaction.Rollback();
                throw new Exception("Error while deleting entity from the database.", e);
            }
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var objects = _dbSet.Where(where).AsEnumerable();
                foreach (var obj in objects)
                    _dbSet.Remove(obj);
                transaction.Commit();
            }
            catch (DbUpdateException e)
            {
                transaction.Rollback();
                throw new Exception("Error while deleting entity from the database.", e);
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

    }
}

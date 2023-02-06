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

        public virtual async Task<IEnumerable<T>> GetAll() => await _dbSet.AsNoTracking().ToListAsync();

        public virtual async Task Insert(T entity)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _dbSet.AddAsync(entity);
                await transaction.CommitAsync();
            }
            catch (DbUpdateException e)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error while inserting entity into the database.", e);
            }
        }

        public virtual async Task Update(T entity)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
                await transaction.CommitAsync();
            }
            catch (DbUpdateException e)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error while updating entity in the database.", e);
            }
        }

        public virtual async Task Update(T entity, Expression<Func<T, bool>> where)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var obj = await _dbSet.FirstOrDefaultAsync(where);
                if (obj == null) return;
                _context.Entry(obj).CurrentValues.SetValues(entity);
                await transaction.CommitAsync();
            }
            catch (DbUpdateException e)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error while updating entity in the database.", e);
            }
        }

        public virtual async Task Delete(T entity)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (_context.Entry(entity).State == EntityState.Detached)
                    _dbSet.Attach(entity);
                _dbSet.Remove(entity);
                await transaction.CommitAsync();
            }
            catch (DbUpdateException e)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error while deleting entity from the database.", e);
            }
        }

        public virtual async Task Delete(Expression<Func<T, bool>> where)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var objects = _dbSet.Where(where).AsEnumerable();
                foreach (var obj in objects)
                    _dbSet.Remove(obj);
                await transaction.CommitAsync();
            }
            catch (DbUpdateException e)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error while deleting entity from the database.", e);
            }
        }

        public virtual async Task<int> Count(Expression<Func<T, bool>> where) => await _dbSet.CountAsync(where);

        public virtual async Task<T?> GetById(object? id) => await _dbSet.FindAsync(id);

        public virtual async Task<IEnumerable<T>> GetList(
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

            return await query.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> where) => await _dbSet.Where(where).ToListAsync();

        public virtual async Task<bool> Any(Expression<Func<T, bool>> where) => await _dbSet.AnyAsync(where);

    }
}

using System.Linq.Expressions;

namespace Domain.Interfaces
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public interface IGenericRepository<T> where T : class
    {
        public Task Insert(T entity);
        public Task Update(T entity);
        public Task Update(T entity, Expression<Func<T, bool>> where);
        public Task Delete(T entity);
        public Task Delete(Expression<Func<T, bool>> where);
        public Task<int> Count(Expression<Func<T, bool>> where);
        public Task<T?> GetById(object? id);
        public Task<IEnumerable<T>> GetList(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "",
            int skip = 0,
            int take = 0);
        public Task<IEnumerable<T>> GetAll();
        public Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> where);
        public Task<bool> Any(Expression<Func<T, bool>> where);
    }
}

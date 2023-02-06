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
        Task Insert(T entity);
        Task Update(T entity);
        Task Update(T entity, Expression<Func<T, bool>> where);
        Task Delete(T entity);
        Task Delete(Expression<Func<T, bool>> where);
        Task<int> Count(Expression<Func<T, bool>> where);
        Task<T?> GetById(object? id);
        Task<IEnumerable<T>> GetList(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "",
            int skip = 0,
            int take = 0);
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> where);
        Task<bool> Any(Expression<Func<T, bool>> where);
    }
}

using Domain.Entities;

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

    public interface ICategoryRepository : IGenericRepository<Category>
    {
        public Task<List<Category>> GetCategories();
        public Task<Category?> GetCategoryById(string? id);
        public Task AddCategory(Category category);
        public Task UpdateCategory(Category category, string id);
        public Task DeleteCategory(string id);
        public Task<bool> CategoryExists(string id);
    }
}

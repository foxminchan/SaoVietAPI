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
        public IEnumerable<Category> GetCategories();
        public Category? GetCategoryById(string? id);
        public void AddCategory(Category category);
        public void UpdateCategory(Category category);
        public void DeleteCategory(string id);
        public bool CategoryExists(string id);
    }
}

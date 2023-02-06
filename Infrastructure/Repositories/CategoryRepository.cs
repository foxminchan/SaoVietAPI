using Domain.Entities;
using Domain.Interfaces;

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

    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Category>> GetCategories()
        {
            var categories = await GetAll();
            return categories.ToList();
        }

        public async Task<Category?> GetCategoryById(string? id) => await GetById(id);

        public async Task AddCategory(Category category) => await Insert(category);

        public async Task UpdateCategory(Category category, string id) => await Update(category, x => x.id == id);

        public async Task DeleteCategory(string id) => await Delete(x => x.id == id);

        public async Task<bool> CategoryExists(string id) => await Any(x => x.id == id);

    }
}

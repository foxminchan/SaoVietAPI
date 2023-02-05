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

        public List<Category> GetCategories() => GetAll().ToList();

        public Category? GetCategoryById(string? id) => GetById(id);

        public void AddCategory(Category category) => Insert(category);

        public void UpdateCategory(Category category, string id) => Update(category, x => x.id == id);

        public void DeleteCategory(string id) => Delete(x => x.id == id);

        public bool CategoryExists(string id) => Any(x => x.id == id);

    }
}

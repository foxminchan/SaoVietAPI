using Application.Common;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Repositories;

namespace Application.Services
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public class CategoryService : BaseService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ApplicationDbContext context, ICache cache) : base(context) => _categoryRepository = new CategoryRepository(context, cache);

        public IEnumerable<Category> GetCategories() => _categoryRepository.GetCategories();

        public Category? GetCategoryById(string? id) => _categoryRepository.GetCategoryById(id);

        public bool CategoryExists(string id) => _categoryRepository.CategoryExists(id);

        public void AddCategory(Category newCategory)
        {
            _categoryRepository.AddCategory(newCategory);
            Save();
        }

        public void UpdateCategory(Category newCategory)
        {
            _categoryRepository.UpdateCategory(newCategory);
            Save();
        }

        public void DeleteCategory(string id)
        {
            _categoryRepository.DeleteCategory(id);
            Save();
        }
    }
}

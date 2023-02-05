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

        public CategoryService(ApplicationDbContext context) : base(context) => _categoryRepository = new CategoryRepository(context);

        public List<Category> GetCategories() => _categoryRepository.GetCategories();

        public Category? GetCategoryById(string? id) => _categoryRepository.GetCategoryById(id);

        public bool CategoryExists(string id) => _categoryRepository.CategoryExists(id);

        public Task AddCategory(Category newCategory)
        {
            _categoryRepository.AddCategory(newCategory);
            return SaveAsync();
        }

        public Task UpdateCategory(Category newCategory, string id)
        {
            _categoryRepository.UpdateCategory(newCategory, id);
            return SaveAsync();
        }

        public Task DeleteCategory(string id)
        {
            _categoryRepository.DeleteCategory(id);
            return SaveAsync();
        }
    }
}

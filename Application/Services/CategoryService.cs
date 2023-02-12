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

        public async Task<List<Category>> GetCategories() => await _categoryRepository.GetCategories();

        public async Task<Category?> GetCategoryById(string? id) => await _categoryRepository.GetCategoryById(id);

        public async Task<bool> CategoryExists(string id) => await _categoryRepository.CategoryExists(id);

        public async Task AddCategory(Category newCategory)
        {
            await _categoryRepository.AddCategory(newCategory);
            await SaveAsync();
        }

        public async Task UpdateCategory(Category newCategory, string id)
        {
            await _categoryRepository.UpdateCategory(newCategory, id);
            await SaveAsync();
        }

        public async Task DeleteCategory(string id)
        {
            await _categoryRepository.DeleteCategory(id);
            await SaveAsync();
        }
    }
}

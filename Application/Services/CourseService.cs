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
    
    public class CourseService : BaseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CourseService(ApplicationDbContext context) : base(context)
        {
            _courseRepository = new CourseRepository(context);
            _categoryRepository = new CategoryRepository(context);
        }

        public async Task<List<Course>> GetCourses() => await _courseRepository.GetCourses();

        public async Task<List<Course>> GetCoursesByNames(string? name) => await _courseRepository.GetCoursesByNames(name);

        public async Task<Course?> GetCourseById(string? id) => await _courseRepository.GetCourseById(id);

        public async Task AddCourse(Course course)
        {
            await _courseRepository.AddCourse(course);
            await SaveAsync();
        }

        public async Task UpdateCourse(Course course, string id)
        {
            await _courseRepository.UpdateCourse(course, id);
            await SaveAsync();
        }

        public async Task DeleteCourse(string id)
        {
            await _courseRepository.DeleteCourse(id);
            await SaveAsync();
        }

        public async Task<bool> CourseExists(string id) => await _courseRepository.CourseExists(id);

        public async Task<bool> IsValidCategoryId(string id) => await _categoryRepository.CategoryExists(id);
    }
}

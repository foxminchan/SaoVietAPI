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
    
    public class CourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CourseService(ApplicationDbContext context, ICache cache)
        {
            _courseRepository = new CourseRepository(context, cache);
            _categoryRepository = new CategoryRepository(context, cache);
        }

        public IEnumerable<Course> GetCourses() => _courseRepository.GetCourses();

        public IEnumerable<Course> GetCoursesByNames(string? name) => _courseRepository.GetCoursesByNames(name);

        public Course? GetCourseById(string? id) => _courseRepository.GetCourseById(id);

        public void AddCourse(Course course) => _courseRepository.AddCourse(course);

        public void UpdateCourse(Course course) => _courseRepository.UpdateCourse(course);

        public void DeleteCourse(string id) => _courseRepository.DeleteCourse(id);

        public bool CourseExists(string id) => _courseRepository.CourseExists(id);

        public bool IsValidCategoryId(string id) => _categoryRepository.CategoryExists(id);
    }
}

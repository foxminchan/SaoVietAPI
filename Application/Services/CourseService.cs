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
        
        public CourseService(ApplicationDbContext context) : base(context) => _courseRepository = new CourseRepository(context);

        public List<Course> GetCourses() => _courseRepository.GetCourses();

        public List<Course> GetCoursesByNames(string? name) => _courseRepository.GetCoursesByNames(name);

        public Course? GetCourseById(string? id) => _courseRepository.GetCourseById(id);

        public Task AddCourse(Course course)
        {
            _courseRepository.AddCourse(course);
            return SaveAsync();
        }

        public Task UpdateCourse(Course course, string id)
        {
            _courseRepository.UpdateCourse(course, id);
            return SaveAsync();
        }

        public Task DeleteCourse(string id)
        {
            _courseRepository.DeleteCourse(id);
            return SaveAsync();
        }

        public bool CourseExists(string id) => _courseRepository.CourseExists(id);
    }
}

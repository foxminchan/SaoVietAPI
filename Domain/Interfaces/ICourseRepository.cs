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

    public interface ICourseRepository : IGenericRepository<Course>
    {
        public Task<List<Course>> GetCourses();
        public Task<List<Course>> GetCoursesByNames(string? name);
        public Task<Course?> GetCourseById(string? id);
        public Task AddCourse(Course course);
        public Task UpdateCourse(Course course, string id);
        public Task DeleteCourse(string id);
        public Task<bool> CourseExists(string id);
    }
}

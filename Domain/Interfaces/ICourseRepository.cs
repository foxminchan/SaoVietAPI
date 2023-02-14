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
        public IEnumerable<Course> GetCourses();
        public IEnumerable<Course> GetCoursesByNames(string? name);
        public Course? GetCourseById(string? id);
        public void AddCourse(Course course);
        public void UpdateCourse(Course course);
        public void DeleteCourse(string id);
        public bool CourseExists(string id);
    }
}

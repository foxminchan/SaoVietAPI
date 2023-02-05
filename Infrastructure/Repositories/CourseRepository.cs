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

    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public List<Course> GetCourses() => GetAll().ToList();

        public List<Course> GetCoursesByNames(string? name) => GetList(filter: x => name != null && x.name != null && x.name.Contains(name)).ToList();

        public Course? GetCourseById(string? id) => id == null ? null : GetById(id);

        public void AddCourse(Course course) => Insert(course);

        public void UpdateCourse(Course course, string id) => Update(course, x => x.id == id);

        public void DeleteCourse(string id) => Delete(x => x.id == id);

        public bool CourseExists(string id) => Any(x => x.id == id);
    }
}

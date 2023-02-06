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

        public async Task<List<Course>> GetCourses()
        {
            var courses = await GetAll();
            return courses.ToList();
        }

        public async Task<List<Course>> GetCoursesByNames(string? name)
        {
            var courses = await GetList(x => name != null && x.name != null && x.name.Contains(name));
            return courses.ToList();
        }

        public async Task<Course?> GetCourseById(string? id) => id == null ? null : await GetById(id);

        public async Task AddCourse(Course course) => await Insert(course);

        public async Task UpdateCourse(Course course, string id) => await Update(course, x => x.id == id);

        public async Task DeleteCourse(string id) => await Delete(x => x.id == id);

        public async Task<bool> CourseExists(string id) => await Any(x => x.id == id);
    }
}

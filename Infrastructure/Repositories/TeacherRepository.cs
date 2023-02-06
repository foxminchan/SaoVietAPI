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

    public class TeacherRepository : GenericRepository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Teacher>> GetTeachers()
        {
            var teachers = await GetAll();
            return teachers.ToList();
        }

        public async Task<List<Teacher>> FindTeacherByName(string name)
        {
            var teachers = await GetList(filter: x => x.fullName != null && x.fullName.Contains(name));
            return teachers.ToList();
        }
        public async Task<Teacher?> GetTeacherById(Guid? id)
        {
            var teacher = await GetById(id);
            return teacher;
        }

        public async Task AddTeacher(Teacher teacher) => await Insert(teacher);

        public async Task UpdateTeacher(Teacher teacher, Guid id) => await Update(teacher, x => x.id == id);

        public async Task DeleteTeacher(Guid id) => await Delete(x => x.id == id);
    }
}
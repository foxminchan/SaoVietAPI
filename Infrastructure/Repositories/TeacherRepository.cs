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
        public TeacherRepository(ApplicationDbContext context, ICache cache) : base(context, cache)
        {
        }

        public IEnumerable<Teacher> GetTeachers() => GetAll();

        public IEnumerable<Teacher> FindTeacherByName(string name) =>
            GetMany(x => x.fullName != null && x.fullName.Contains(name));

        public Teacher? GetTeacherById(Guid? id) => GetById(id);

        public void AddTeacher(Teacher teacher) => Insert(teacher);

        public void UpdateTeacher(Teacher teacher) => Update(teacher);

        public void DeleteTeacher(Guid id) => Delete(x => x.id == id);
    }
}
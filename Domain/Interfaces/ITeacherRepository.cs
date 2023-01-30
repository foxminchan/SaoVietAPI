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

    public interface ITeacherRepository : IGenericRepository<Teacher>
    {
        public List<Teacher> GetTeachers();
        public void AddTeacher(Teacher teacher);
        public void UpdateTeacher(Teacher teacher, Guid id);
        public void DeleteTeacher(Guid id);
        public List<Teacher> FindTeacherByName(string name);
        public Teacher? GetTeacherById(Guid? id);
    }
}
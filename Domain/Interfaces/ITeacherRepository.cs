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
        public Task<List<Teacher>> GetTeachers();
        public Task<List<Teacher>> FindTeacherByName(string name);
        public Task<Teacher?> GetTeacherById(Guid? id);
        public Task AddTeacher(Teacher teacher);
        public Task UpdateTeacher(Teacher teacher, Guid id);
        public Task DeleteTeacher(Guid id);
    }
}
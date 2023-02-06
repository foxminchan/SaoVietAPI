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

    public interface IClassRepository : IGenericRepository<Class>
    {
        public Task<List<Class>> GetClasses();
        public Task<List<Class>> FindClassByName(string? name);
        public Task<List<Class>> GetClassesByStatus(string? status);
        public Task<List<Class>> FindClassByTeacher(Guid? teacherId);
        public Task<Class?> FindClassById(string? id);
        public Task AddClass(Class entity);
        public Task UpdateClass(Class entity, string id);
        public Task DeleteClass(string id);
        public Task<bool> ClassExists(string? id);
    }
}

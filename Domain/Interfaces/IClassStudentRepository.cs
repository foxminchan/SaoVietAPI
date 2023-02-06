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

    public interface IClassStudentRepository : IGenericRepository<ClassStudent>
    {
        public Task<int> CountStudentInClass(string? classId);
        public Task<int> CountClassByStudent(Guid? studentId);
        public Task<IEnumerable<Guid?>> GetAllStudentIdByClassId(string? classId);
        public Task<IEnumerable<string?>> GetAllClassIdByStudentId(Guid? studentId);
        public Task AddClassStudent(ClassStudent classStudent);
        public Task DeleteClassStudent(ClassStudent classStudent);
        public Task<bool> IsExistClassStudent(string? classId, Guid? studentId);
    }
}

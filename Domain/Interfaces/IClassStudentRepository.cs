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
        public int CountStudentInClass(string? classId);
        public int CountClassByStudent(Guid? studentId);
        public IEnumerable<Guid?> GetAllStudentIdByClassId(string? classId);
        public IEnumerable<string?> GetAllClassIdByStudentId(Guid? studentId);
        public void AddClassStudent(ClassStudent classStudent);
        public void DeleteClassStudent(ClassStudent classStudent);
        public bool IsExistClassStudent(string? classId, Guid? studentId);
    }
}

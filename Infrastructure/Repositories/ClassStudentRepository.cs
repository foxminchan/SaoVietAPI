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

    public class ClassStudentRepository : GenericRepository<ClassStudent>, IClassStudentRepository
    {
        public ClassStudentRepository(ApplicationDbContext context, ICache cache) : base(context, cache)
        {
        }

        public int CountStudentInClass(string? classId) => Count(x => x.classId == classId);

        public int CountClassByStudent(Guid? studentId) => Count(x => x.studentId == studentId);

        public IEnumerable<Guid?> GetAllStudentIdByClassId(string? classId) =>
            GetMany(x => x.classId == classId).Select(x => x.studentId);

        public IEnumerable<string?> GetAllClassIdByStudentId(Guid? studentId) =>
            GetMany(x => x.studentId == studentId).Select(x => x.classId);

        public void AddClassStudent(ClassStudent classStudent) => Insert(classStudent);

        public void DeleteClassStudent(ClassStudent classStudent) => Delete(classStudent);

        public bool IsExistClassStudent(string? classId, Guid? studentId) => Any(x => x.classId == classId && x.studentId == studentId);

    }
}

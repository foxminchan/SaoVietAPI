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

        public async Task<int> CountStudentInClass(string? classId) => await Count(x => x.classId == classId);

        public async Task<int> CountClassByStudent(Guid? studentId) => await Count(x => x.studentId == studentId);

        public async Task<IEnumerable<Guid?>> GetAllStudentIdByClassId(string? classId)
        {
            var students = await GetList(x => x.classId == classId);
            return students.Select(x => x.studentId).ToList();
        }

        public async Task<IEnumerable<string?>> GetAllClassIdByStudentId(Guid? studentId)
        {
            var classes = await GetList(x => x.studentId == studentId);
            return classes.Select(x => x.classId).ToList();
        }

        public async Task AddClassStudent(ClassStudent classStudent) => await Insert(classStudent);

        public async Task DeleteClassStudent(ClassStudent classStudent) => await Delete(classStudent);

        public async Task<bool> IsExistClassStudent(string? classId, Guid? studentId) => await Any(x => x.classId == classId && x.studentId == studentId);

    }
}

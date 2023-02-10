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

    public class AttendanceRepository : GenericRepository<Attendance>, IAttendanceRepository
    {
        public AttendanceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Attendance>> GetAllAttendance()
        {
            var result = await GetAll();
            return result.ToList();
        }

        public async Task<List<Attendance>> GetAttendanceById(string? classId, string? lessonId)
        {
            var result = await GetList(filter: x => x.classId == classId && x.lessonId == lessonId);
            return result.ToList();
        }

        public async Task<List<Attendance>> GetAttendanceByClassId(string? classId)
        {
            var result = await GetList(filter: x => x.classId == classId);
            return result.ToList();
        }

        public async Task<List<Attendance>> SortByAttendance()
        {
            var result = await GetList(orderBy: x => x.OrderBy(y => y.attendance));
            return result.ToList();
        }

        public async Task AddAttendance(Attendance attendance) => await Insert(attendance);

        public async Task UpdateAttendance(Attendance attendance, string classId, string lessonId) => await Update(attendance, x => x.classId == classId && x.lessonId == lessonId);

        public async Task DeleteAttendance(string classId, string lessonId) => await Delete(x => x.classId == classId && x.lessonId == lessonId);

        public async Task<bool> IsAttendanceExist(string classId, string lessonId) =>
            await Any(x => x.classId == classId && x.lessonId == lessonId);
    }
}

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
        public AttendanceRepository(ApplicationDbContext context, ICache cache) : base(context, cache)
        {
        }

        public IEnumerable<Attendance> GetAllAttendance() => GetAll();

        public IEnumerable<Attendance> GetAttendanceById(string? classId, string? lessonId) =>
            GetList(filter: x => x.classId == classId && x.lessonId == lessonId);

        public IEnumerable<Attendance> GetAttendanceByClassId(string? classId) =>
            GetList(filter: x => x.classId == classId);

        public IEnumerable<Attendance> SortByAttendance() => GetList(orderBy: x => x.OrderBy(y => y.attendance));

        public void AddAttendance(Attendance attendance) => Insert(attendance);

        public void UpdateAttendance(Attendance attendance) => Update(attendance);

        public void DeleteAttendance(string classId, string lessonId) => Delete(x => x.classId == classId && x.lessonId == lessonId);

        public bool IsAttendanceExist(string classId, string lessonId) => Any(x => x.classId == classId && x.lessonId == lessonId);
    }
}

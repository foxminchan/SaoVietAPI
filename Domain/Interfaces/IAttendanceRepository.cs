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

    public interface IAttendanceRepository : IGenericRepository<Attendance>
    {
        public IEnumerable<Attendance> GetAllAttendance();
        public IEnumerable<Attendance> GetAttendanceById(string? classId, string? lessonId);
        public IEnumerable<Attendance> GetAttendanceByClassId(string? classId);
        public IEnumerable<Attendance> SortByAttendance();
        public void AddAttendance(Attendance attendance);
        public void UpdateAttendance(Attendance attendance);
        public void DeleteAttendance(string classId, string lessonId);
        public bool IsAttendanceExist(string classId, string lessonId);
    }
}

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
        public Task<List<Attendance>> GetAllAttendance();
        public Task<List<Attendance>> GetAttendanceById(string? classId, string? lessonId);
        public Task<List<Attendance>> GetAttendanceByClassId(string? classId);
        public Task<List<Attendance>> SortByAttendance();
        public Task AddAttendance(Attendance attendance);
        public Task UpdateAttendance(Attendance attendance, string classId, string lessonId);
        public Task DeleteAttendance(string classId, string lessonId);
    }
}

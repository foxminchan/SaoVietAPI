using Application.Common;
using Domain.Entities;
using Domain.Interfaces;
using Hangfire;
using Infrastructure;
using Infrastructure.Repositories;

namespace Application.Services
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public class AttendanceService : BaseService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IClassRepository _classRepository;

        public AttendanceService(ApplicationDbContext context, ICache cache) : base(context)
        {
            _attendanceRepository = new AttendanceRepository(context, cache);
            _lessonRepository = new LessonRepository(context, cache);
            _classRepository = new ClassRepository(context, cache);
        }

        public IEnumerable<Attendance> GetAllAttendance() => _attendanceRepository.GetAllAttendance();

        public IEnumerable<Attendance> GetAttendanceById(string? classId, string? lessonId) => _attendanceRepository.GetAttendanceById(classId, lessonId);

        public IEnumerable<Attendance> GetAttendanceByClassId(string? classId) => _attendanceRepository.GetAttendanceByClassId(classId);

        public IEnumerable<Attendance> SortByAttendance() => _attendanceRepository.SortByAttendance();

        public void AddAttendance(Attendance attendance)
        {
            _attendanceRepository.AddAttendance(attendance);
            Save();
        }

        public void UpdateAttendance(Attendance attendance)
        {
            _attendanceRepository.UpdateAttendance(attendance);
            Save();
        }

        public void DeleteAttendance(string classId, string lessonId)
        {
            _attendanceRepository.DeleteAttendance(classId, lessonId);
            Save();
        }

        public bool CheckLessonExists(string lessonId) => _lessonRepository.LessonExists(lessonId);

        public bool CheckClassExists(string classId) => _classRepository.ClassExists(classId);

        public bool IsAttendanceExist(string classId, string lessonId) => _attendanceRepository.IsAttendanceExist(classId, lessonId);
    }
}

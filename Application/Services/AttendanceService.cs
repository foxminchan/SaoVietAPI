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

        public AttendanceService(ApplicationDbContext context, ICache cache) : base(context, cache)
        {
            _attendanceRepository = new AttendanceRepository(context, cache);
            _lessonRepository = new LessonRepository(context, cache);
            _classRepository = new ClassRepository(context, cache);
        }

        public async Task<List<Attendance>> GetAllAttendance() => await _attendanceRepository.GetAllAttendance();

        public async Task<List<Attendance>> GetAttendanceById(string? classId, string? lessonId) => await _attendanceRepository.GetAttendanceById(classId, lessonId);

        public async Task<List<Attendance>> GetAttendanceByClassId(string? classId) => await _attendanceRepository.GetAttendanceByClassId(classId);

        public async Task<List<Attendance>> SortByAttendance() => await _attendanceRepository.SortByAttendance();

        public async Task AddAttendance(Attendance attendance)
        {
            await _attendanceRepository.AddAttendance(attendance);
            await SaveAsync();
        }

        public async Task UpdateAttendance(Attendance attendance, string classId, string lessonId)
        {
            await _attendanceRepository.UpdateAttendance(attendance, classId, lessonId);
            await SaveAsync();
        }

        public async Task DeleteAttendance(string classId, string lessonId)
        {
            await _attendanceRepository.DeleteAttendance(classId, lessonId);
            await SaveAsync();
        }

        public async Task<bool> CheckLessonExists(string lessonId) => await _lessonRepository.LessonExists(lessonId);

        public async Task<bool> CheckClassExists(string classId) => await _classRepository.ClassExists(classId);

        public async Task<bool> IsAttendanceExist(string classId, string lessonId) => await _attendanceRepository.IsAttendanceExist(classId, lessonId);
    }
}

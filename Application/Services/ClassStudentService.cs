using Application.Common;
using Domain.Entities;
using Domain.Interfaces;
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

    public class ClassStudentService : BaseService
    {
        private readonly IClassStudentRepository _classStudentRepository;
        
        public ClassStudentService(ApplicationDbContext context) : base(context) => _classStudentRepository = new ClassStudentRepository(context);

        public async Task<int> CountStudentInClass(string? classId) => await _classStudentRepository.CountStudentInClass(classId);
        
        public async Task<int> CountClassByStudent(Guid? studentId) => await _classStudentRepository.CountClassByStudent(studentId);

        public async Task<IEnumerable<Guid?>> GetAllStudentIdByClassId(string? classId)
        {
            var studentIds = await _classStudentRepository.GetAllStudentIdByClassId(classId);
            return studentIds;
        }

        public async Task<IEnumerable<string?>> GetAllClassIdByStudentId(Guid? studentId)
        {
            var classIds = await _classStudentRepository.GetAllClassIdByStudentId(studentId);
            return classIds;
        } 
        
        public async Task AddClassStudent(ClassStudent classStudent) => await _classStudentRepository.AddClassStudent(classStudent);

        public async Task DeleteClassStudent(ClassStudent classStudent) => await _classStudentRepository.DeleteClassStudent(classStudent);

        public async Task<bool> IsExistClassStudent(string? classId, Guid? studentId) => await _classStudentRepository.IsExistClassStudent(classId, studentId);
    }
}

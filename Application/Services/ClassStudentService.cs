using Application.Common;
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

        public int CountStudentInClass(string? classId) => _classStudentRepository.CountStudentInClass(classId);
        
        public int CountClassByStudent(Guid? studentId) => _classStudentRepository.CountClassByStudent(studentId);
        
        public List<Guid?> GetAllStudentIdByClassId(string? classId) => _classStudentRepository.GetAllStudentIdByClassId(classId);
        
        public List<string?> GetAllClassIdByStudentId(Guid? studentId) => _classStudentRepository.GetAllClassIdByStudentId(studentId);
    }
}

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
    
    public class ClassService : BaseService
    {
        private readonly IClassRepository _classRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IClassStudentRepository _classStudentRepository;
        private readonly IStudentRepository _studentRepository;

        public ClassService(ApplicationDbContext context, ICache cache) : base(context, cache)
        {
            _classRepository = new ClassRepository(context, cache);
            _teacherRepository = new TeacherRepository(context, cache);
            _branchRepository = new BranchRepository(context, cache);
            _classStudentRepository = new ClassStudentRepository(context, cache);
            _studentRepository = new StudentRepository(context, cache);
        }

        public async Task<List<Class>> GetClasses() => await _classRepository.GetClasses();

        public async Task<List<Class>> FindClassByName(string? name) => await _classRepository.FindClassByName(name);

        public async Task<List<Class>> GetClassesByStatus(string? status) => await _classRepository.GetClassesByStatus(status);

        public async Task<List<Class>> FindClassByTeacher(Guid? teacherId) => await _classRepository.FindClassByTeacher(teacherId);

        public async Task<Class?> FindClassById(string? id) => await _classRepository.FindClassById(id);

        public async Task AddClass(Class newClass)
        {
            await _classRepository.AddClass(newClass);
            await SaveAsync();
        }

        public async Task UpdateClass(Class newClass, string id)
        {
            await _classRepository.UpdateClass(newClass, id);
            await SaveAsync();
        }

        public async Task DeleteClass(string id)
        {
            await _classRepository.DeleteClass(id);
            await SaveAsync();
        }

        public async Task<IEnumerable<Teacher?>> GetTeachers() => await _teacherRepository.GetTeachers();

        public async Task<IEnumerable<Branch?>> GetBranches() => await _branchRepository.GetBranches();

        public async Task<int> CountStudentInClass(string? classId) => await _classStudentRepository.CountStudentInClass(classId);

        public async Task<List<Student?>> GetStudentsInClass(string? classId)
        {
            var studentIds = await _classStudentRepository.GetAllStudentIdByClassId(classId);
            return Task.WhenAll(studentIds.Select(_studentRepository.GetStudentById)).Result.ToList();
        }

        public async Task<bool> CheckStudentInClass(string? classId, Guid? studentId) => await _classStudentRepository.IsExistClassStudent(classId, studentId);

        public async Task DeleteStudentFromClass(ClassStudent classStudent)
        {
            await _classStudentRepository.DeleteClassStudent(classStudent);
            await SaveAsync();
        }

        public async Task<bool> CheckClassIdExist(string? classId) => await _classRepository.ClassExists(classId);
    }
}

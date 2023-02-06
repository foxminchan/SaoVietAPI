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
        private readonly ApplicationDbContext _context;
        private readonly IClassRepository _classRepository;

        public ClassService(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _classRepository = new ClassRepository(context);
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

        public async Task<IEnumerable<Teacher?>> GetTeachers()
        {
            var teacherService = new TeacherService(_context);
            var teacherIds = await _classRepository.GetClasses();
            return Task.WhenAll(teacherIds.Select(x => teacherService.GetTeacherById(x.teacherId))).Result;
        }

        public async Task<IEnumerable<Branch?>> GetBranches()
        {
            var branchService = new BranchService(_context);
            var branchIds = await _classRepository.GetClasses();
            return Task.WhenAll(branchIds.Select(x => branchService.GetBranchById(x.branchId))).Result;
        }

        public async Task<int> CountStudentInClass(string? classId)
        {
            var classStudentService = new ClassStudentService(_context);
            return await classStudentService.CountStudentInClass(classId);
        }

        public async Task<List<Student?>> GetStudentsInClass(string? classId)
        {
            var studentService = new StudentService(_context);
            var studentIds = await new ClassStudentService(_context).GetAllStudentIdByClassId(classId);
            return Task.WhenAll(studentIds.Select(studentService.GetStudentById)).Result.ToList();
        }

        public async Task<bool> CheckStudentInClass(string? classId, Guid? studentId) => await new ClassStudentService(_context).IsExistClassStudent(classId, studentId);

        public async Task DeleteStudentFromClass(ClassStudent classStudent)
        {
            await new ClassStudentService(_context).DeleteClassStudent(classStudent);
            await SaveAsync();
        }

        public async Task<bool> CheckClassIdExist(string? classId) => await _classRepository.ClassExists(classId);
    }
}

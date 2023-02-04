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

        public List<Class> GetClasses() => _classRepository.GetClasses();

        public List<Class> FindClassByName(string? name) => _classRepository.FindClassByName(name);

        public List<Class> GetClassesByStatus(string? status) => _classRepository.GetClassesByStatus(status);

        public List<Class> FindClassByTeacher(Guid? teacherId) => _classRepository.FindClassByTeacher(teacherId);

        public Class? FindClassById(string? id) => _classRepository.FindClassById(id);

        public Task AddClass(Class newClass)
        {
            _classRepository.AddClass(newClass);
            return SaveAsync();
        }

        public Task UpdateClass(Class newClass, string id)
        {
            _classRepository.UpdateClass(newClass, id);
            return SaveAsync();
        }

        public Task DeleteClass(string id)
        {
            _classRepository.DeleteClass(id);
            return SaveAsync();
        }

        public IEnumerable<Teacher?> GetTeachers()
        {
            var teacherService = new TeacherService(_context);
            var teacherIds = _classRepository.GetClasses().Select(x => x.teacherId).ToList();
            return teacherIds.Select(teacherService.GetTeacherById).ToList();
        }

        public IEnumerable<Branch?> GetBranches()
        {
            var branchService = new BranchService(_context);
            var branchIds = _classRepository.GetClasses().Select(x => x.branchId).ToList();
            return branchIds.Select(branchService.GetBranchById).ToList();
        }

        public int CountStudentInClass(string? classId) => new ClassStudentService(_context).CountStudentInClass(classId);

        public List<Student?> GetStudentsInClass(string? classId)
        {
            var studentService = new StudentService(_context);
            var studentIds = new ClassStudentService(_context).GetAllStudentIdByClassId(classId);
            return studentIds.Select(studentService.GetStudentById).ToList();
        }

        public void DeleteStudentFromClass(ClassStudent classStudent) => new ClassStudentService(_context).DeleteClassStudent(classStudent);
        
    }
}

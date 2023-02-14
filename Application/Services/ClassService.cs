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

    public class ClassService
    {
        private readonly IClassRepository _classRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IClassStudentRepository _classStudentRepository;
        private readonly IStudentRepository _studentRepository;

        public ClassService(ApplicationDbContext context, ICache cache)
        {
            _classRepository = new ClassRepository(context, cache);
            _teacherRepository = new TeacherRepository(context, cache);
            _branchRepository = new BranchRepository(context, cache);
            _classStudentRepository = new ClassStudentRepository(context, cache);
            _studentRepository = new StudentRepository(context, cache);
        }

        public IEnumerable<Class> GetClasses() => _classRepository.GetClasses();

        public IEnumerable<Class> FindClassByName(string? name) => _classRepository.FindClassByName(name);

        public IEnumerable<Class> GetClassesByStatus(string? status) => _classRepository.GetClassesByStatus(status);

        public IEnumerable<Class> FindClassByTeacher(Guid? teacherId) => _classRepository.FindClassByTeacher(teacherId);

        public Class? FindClassById(string? id) => _classRepository.FindClassById(id);

        public void AddClass(Class newClass) => _classRepository.AddClass(newClass);

        public void UpdateClass(Class newClass) => _classRepository.UpdateClass(newClass);

        public void DeleteClass(string id) => _classRepository.DeleteClass(id);

        public IEnumerable<Teacher?> GetTeachers() => _teacherRepository.GetTeachers();

        public IEnumerable<Branch?> GetBranches() => _branchRepository.GetBranches();

        public int CountStudentInClass(string? classId) => _classStudentRepository.CountStudentInClass(classId);

        public IEnumerable<Student?> GetStudentsInClass(string? classId) => _classStudentRepository
            .GetAllStudentIdByClassId(classId).Select(studentId => _studentRepository.GetById(studentId));

        public bool CheckStudentInClass(string? classId, Guid? studentId) => _classStudentRepository.IsExistClassStudent(classId, studentId);

        public void DeleteStudentFromClass(ClassStudent classStudent) => _classStudentRepository.DeleteClassStudent(classStudent);

        public bool CheckClassIdExist(string? classId) => _classRepository.ClassExists(classId);
    }
}

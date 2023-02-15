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

    public class StudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IClassStudentRepository _classStudentRepository;
        private readonly IClassRepository _classRepository;

        public StudentService(ApplicationDbContext context, ICache cache)
        {
            _studentRepository = new StudentRepository(context, cache);
            _classRepository = new ClassRepository(context, cache);
            _classStudentRepository = new ClassStudentRepository(context, cache);
        }

        public IEnumerable<Student> GetStudents() => _studentRepository.GetStudents();

        public IEnumerable<Student> GetStudentsByNames(string? name) => _studentRepository.GetStudentsByNames(name);

        public IEnumerable<Student> GetStudentsByPhone(string? phone) => _studentRepository.GetStudentsByPhone(phone);

        public Student? GetStudentById(Guid? id) => _studentRepository.GetStudentById(id);

        public void AddStudent(Student student) => _studentRepository.AddStudent(student);

        public void UpdateStudent(Student student) => _studentRepository.UpdateStudent(student);

        public void DeleteStudent(Guid id) => _studentRepository.DeleteStudent(id);

        public int CountClassByStudent(Guid? studentId) => _classStudentRepository.CountClassByStudent(studentId);

        public IEnumerable<Class?> GetClassesByStudentId(Guid? studentId) => _classStudentRepository
            .GetAllClassIdByStudentId(studentId).Select(x => _classRepository.FindClassById(x));

        public bool CheckStudentExists(Guid? id) => _studentRepository.StudentExists(id);

        public bool CheckClassExists(string? id) => _classRepository.ClassExists(id);

        public bool IsAlreadyInClass(Guid? studentId, string? classId) => _classStudentRepository.IsExistClassStudent(classId, studentId);

        public void AddClassStudent(ClassStudent classStudent) => _classStudentRepository.AddClassStudent(classStudent);
    }
}

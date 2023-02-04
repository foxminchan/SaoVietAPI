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

    public class StudentService : BaseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStudentRepository _studentRepository;
        
        public StudentService(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _studentRepository = new StudentRepository(context);
        }

        public List<Student> GetStudents() => _studentRepository.GetStudents();

        public List<Student> GetStudentsByNames(string? name) => _studentRepository.GetStudentsByNames(name);

        public List<Student> GetStudentsByPhone(string? phone) => _studentRepository.GetStudentsByPhone(phone);

        public Student? GetStudentById(Guid? id) => _studentRepository.GetStudentById(id);

        public Task AddStudent(Student student)
        {
            _studentRepository.AddStudent(student);
            return SaveAsync();
        }

        public Task UpdateStudent(Student student, Guid id)
        {
            _studentRepository.UpdateStudent(student, id);
            return SaveAsync();
        }

        public Task DeleteStudent(Guid id)
        {
            _studentRepository.DeleteStudent(id);
            return SaveAsync();
        }

        public int CountClassByStudent(Guid? studentId) => new ClassStudentService(_context).CountClassByStudent(studentId);

        public IEnumerable<Class?> GetClassesByStudentId(Guid? studentId)
        {
            var classService = new ClassService(_context);
            var classIds = new ClassStudentService(_context).GetAllClassIdByStudentId(studentId);
            return classIds.Select(classService.FindClassById).ToList();
        }

        public void AddClassStudent(ClassStudent classStudent) => new ClassStudentService(_context).AddClassStudent(classStudent);
    }
}

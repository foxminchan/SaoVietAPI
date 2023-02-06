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

        public async Task<List<Student>> GetStudents() => await _studentRepository.GetStudents();

        public async Task<List<Student>> GetStudentsByNames(string? name) => await _studentRepository.GetStudentsByNames(name);

        public async Task<List<Student>> GetStudentsByPhone(string? phone) => await _studentRepository.GetStudentsByPhone(phone);

        public async Task<Student?> GetStudentById(Guid? id) => await _studentRepository.GetStudentById(id);

        public async Task AddStudent(Student student)
        {
            await _studentRepository.AddStudent(student);
            await SaveAsync();
        }

        public async Task UpdateStudent(Student student, Guid id)
        {
            await _studentRepository.UpdateStudent(student, id);
            await SaveAsync();
        }

        public async Task DeleteStudent(Guid id)
        {
            await _studentRepository.DeleteStudent(id);
            await SaveAsync();
        }

        public async Task<int> CountClassByStudent(Guid? studentId) => await new ClassStudentService(_context).CountClassByStudent(studentId);

        public async Task<IEnumerable<Class?>> GetClassesByStudentId(Guid? studentId)
        {
            var classService = new ClassService(_context);
            var classIds = await new ClassStudentService(_context).GetAllClassIdByStudentId(studentId);
            return await Task.WhenAll(classIds.Select(classService.FindClassById));
        }

        public async Task<bool> CheckStudentExists(Guid? id) => await _studentRepository.StudentExists(id);

        public async Task<bool> CheckClassExists(string? id) => await new ClassService(_context).CheckClassIdExist(id);

        public async Task<bool> IsAlreadyInClass(Guid? studentId, string? classId) => await new ClassStudentService(_context).IsExistClassStudent(classId, studentId);

        public async Task AddClassStudent(ClassStudent classStudent)
        {
            await new ClassStudentService(_context).AddClassStudent(classStudent);
            await SaveAsync();
        }
    }
}

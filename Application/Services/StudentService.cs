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
        private readonly IStudentRepository _studentRepository;
        private readonly IClassStudentRepository _classStudentRepository;
        private readonly IClassRepository _classRepository;

        public StudentService(ApplicationDbContext context) : base(context)
        {
            _studentRepository = new StudentRepository(context);
            _classRepository = new ClassRepository(context);
            _classStudentRepository = new ClassStudentRepository(context);
            
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

        public async Task<int> CountClassByStudent(Guid? studentId) => await _classStudentRepository.CountClassByStudent(studentId);

        public async Task<IEnumerable<Class?>> GetClassesByStudentId(Guid? studentId)
        {
            var classIds = await _classStudentRepository.GetAllClassIdByStudentId(studentId);
            return await Task.WhenAll(classIds.Select(_classRepository.FindClassById));
        }

        public async Task<bool> CheckStudentExists(Guid? id) => await _studentRepository.StudentExists(id);

        public async Task<bool> CheckClassExists(string? id) => await _classRepository.ClassExists(id);

        public async Task<bool> IsAlreadyInClass(Guid? studentId, string? classId) => await _classStudentRepository.IsExistClassStudent(classId, studentId);

        public async Task AddClassStudent(ClassStudent classStudent)
        {
            await _classStudentRepository.AddClassStudent(classStudent);
            await SaveAsync();
        }
    }
}

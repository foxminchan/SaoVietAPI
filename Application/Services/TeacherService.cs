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

    public class TeacherService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITeacherRepository _teacherRepository;

        public TeacherService(ApplicationDbContext context, ICache cache)
        {
            _context = context;
            _teacherRepository = new TeacherRepository(_context, cache);
        }

        public IEnumerable<string> GetAllId() => _context.GetAllId();

        public IEnumerable<Teacher> GetTeachers() => _teacherRepository.GetTeachers();

        public IEnumerable<Teacher> FindTeacherByName(string name) => _teacherRepository.FindTeacherByName(name);

        public Teacher? GetTeacherById(Guid? id) => _teacherRepository.GetTeacherById(id);

        public void AddTeacher(Teacher newTeacher) => _teacherRepository.AddTeacher(newTeacher);

        public void UpdateTeacher(Teacher newTeacher) => _teacherRepository.UpdateTeacher(newTeacher);

        public void DeleteTeacher(Guid id) => _teacherRepository.DeleteTeacher(id);
    }
}

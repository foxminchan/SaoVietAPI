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

    public class TeacherService : BaseService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITeacherRepository _teacherRepository;

        public TeacherService(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _teacherRepository = new TeacherRepository(_context);
        }

        public async Task<IEnumerable<string>> GetAllId() => await  _context.GetAllId();

        public async Task<List<Teacher>> GetTeachers() => await _teacherRepository.GetTeachers();

        public async Task<List<Teacher>> FindTeacherByName(string name) => await _teacherRepository.FindTeacherByName(name);

        public async Task<Teacher?> GetTeacherById(Guid? id) => await _teacherRepository.GetTeacherById(id);

        public async Task AddTeacher(Teacher newTeacher)
        {
            await _teacherRepository.AddTeacher(newTeacher);
            await SaveAsync();
        }

        public async Task UpdateTeacher(Teacher newTeacher, Guid id)
        {
            await _teacherRepository.UpdateTeacher(newTeacher, id);
            await SaveAsync();
        }

        public async Task DeleteTeacher(Guid id)
        {
            await _teacherRepository.DeleteTeacher(id);
            await SaveAsync();
        }
    }
}

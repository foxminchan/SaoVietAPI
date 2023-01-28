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
        readonly ITeacherRepository _teacherRepository;

        public TeacherService(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _teacherRepository = new TeacherRepository(_context);
        }

        public List<string> GetAllId() => _context.Users.Select(x => x.Id).ToList();

        public List<Teacher> GetTeachers() => _teacherRepository.GetTeachers();

        public Task AddTeacher(Teacher newTeacher)
        {
            _teacherRepository.AddTeacher(newTeacher);
            return SaveAsync();
        }

        public Task UpdateTeacher(Teacher newTeacher, Guid id)
        {
            _teacherRepository.UpdateTeacher(newTeacher, id);
            return SaveAsync();
        }

        public Task DeleteTeacher(Guid id)
        {
            _teacherRepository.DeleteTeacher(id);
            return SaveAsync();
        }
    }
}

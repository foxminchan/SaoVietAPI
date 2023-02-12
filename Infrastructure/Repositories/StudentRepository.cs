using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepository(ApplicationDbContext context, ICache cache) : base(context, cache)
        {
        }

        public async Task<List<Student>> GetStudents()
        {
            var students = await GetAll();
            return students.ToList();
        }

        public async Task<List<Student>> GetStudentsByNames(string? name)
        {
            var students = await GetList(filter: s => name != null && s.fullName != null && s.fullName.Contains(name));
            return students.ToList();
        }

        public async Task<List<Student>> GetStudentsByPhone(string? phone)
        {
            var students = await GetList(filter: s => phone != null && s.phone != null && s.phone.Contains(phone));
            return students.ToList();
        }

        public async Task<Student?> GetStudentById(Guid? id) => await GetById(id);

        public async Task AddStudent(Student student) => await Insert(student);

        public async Task UpdateStudent(Student student, Guid id) => await Update(student, x => x.id == id);

        public async Task DeleteStudent(Guid id) => await Delete(x => x.id == id);

        public async Task<bool> StudentExists(Guid? id) => await Any(x => x.id == id);
    }
}

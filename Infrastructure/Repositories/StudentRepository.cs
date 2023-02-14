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

        public IEnumerable<Student> GetStudents() => GetAll();

        public IEnumerable<Student> GetStudentsByNames(string? name) => GetMany(x => name != null && x.fullName != null && x.fullName.Contains(name));

        public IEnumerable<Student> GetStudentsByPhone(string? phone) => GetMany(x => phone != null && x.phone != null && x.phone.Contains(phone));

        public Student? GetStudentById(Guid? id) => GetById(id);

        public void AddStudent(Student student) => Insert(student);

        public void UpdateStudent(Student student) => Update(student);

        public void DeleteStudent(Guid id) => Delete(x => x.id == id);

        public bool StudentExists(Guid? id) => Any(x => x.id == id);
    }
}

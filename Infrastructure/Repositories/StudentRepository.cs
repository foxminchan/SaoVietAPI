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
        public StudentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public List<Student> GetStudents() => GetAll().ToList();

        public List<Student> GetStudentsByNames(string? name) => GetList(filter: s => name != null && s.fullName != null && s.fullName.Contains(name)).ToList();

        public List<Student> GetStudentsByPhone(string? phone) => GetList(filter: s => phone != null && s.phone != null && s.phone.Contains(phone)).ToList();

        public Student? GetStudentById(Guid? id) => GetById(id);

        public void AddStudent(Student student) => Insert(student);

        public void UpdateStudent(Student student, Guid id) => Update(student, x => x.id == id);

        public void DeleteStudent(Guid id) => Delete(x => x.id == id);

        public bool StudentExists(Guid? id) => Any(x => x.id == id);
    }
}

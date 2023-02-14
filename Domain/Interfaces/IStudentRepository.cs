using Domain.Entities;

namespace Domain.Interfaces
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public interface IStudentRepository : IGenericRepository<Student>
    {
        public IEnumerable<Student> GetStudents();
        public IEnumerable<Student> GetStudentsByNames(string? name);
        public IEnumerable<Student> GetStudentsByPhone(string? phone);
        public Student? GetStudentById(Guid? id);
        public void AddStudent(Student student);
        public void UpdateStudent(Student student);
        public void DeleteStudent(Guid id);
        public bool StudentExists(Guid? id);
    }
}

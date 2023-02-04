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
        public List<Student> GetStudents();
        public List<Student> GetStudentsByNames(string? name);
        public List<Student> GetStudentsByPhone(string? phone);
        public Student? GetStudentById(Guid? id);
        public void AddStudent(Student student);
        public void UpdateStudent(Student student, Guid id);
        public void DeleteStudent(Guid id);
        public bool StudentExists(Guid? id);
    }
}

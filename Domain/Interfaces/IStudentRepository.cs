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
        public Task<List<Student>> GetStudents();
        public Task<List<Student>> GetStudentsByNames(string? name);
        public Task<List<Student>> GetStudentsByPhone(string? phone);
        public Task<Student?> GetStudentById(Guid? id);
        public Task AddStudent(Student student);
        public Task UpdateStudent(Student student, Guid id);
        public Task DeleteStudent(Guid id);
        public Task<bool> StudentExists(Guid? id);
    }
}

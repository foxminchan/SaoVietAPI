namespace Domain.Entities
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public class Student
    {
        public Guid? id { get; set; }
        public string? fullName { get; set; }
        public string? dob { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public ICollection<ClassStudent>? classStudents { get; set; }
    }
}

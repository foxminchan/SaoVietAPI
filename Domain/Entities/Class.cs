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

    public class Class
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? startDate { get; set; }
        public string? endDate { get; set; }
        public Guid? teacherId { get; set; }
        public Teacher? teacher { get; set; }
        public string? branchId { get; set; }
        public Branch? branch { get; set; }
        public ICollection<ClassStudent>? classStudents { get; set; }
        public ICollection<Attendance>? attendances { get; set; }
    }
}

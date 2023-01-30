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

    public class Attendance
    {
        public string? classId { get; set; }
        public Class? @class { get; set; }
        public string? lessonId { get; set; }
        public Lesson? lesson { get; set; }
        public string? date { get; set; }
        public string? comment { get; set; }
        public byte? evaluation { get; set; }
        public byte? attendance { get; set; }
    }
}

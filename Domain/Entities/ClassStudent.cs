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

    public class ClassStudent
    {
        public string? classId { get; set; }
        public Class? @class { get; set; }
        public Guid? studentId { get; set; }
        public Student? student { get; set; }
    }
}

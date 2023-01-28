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

    public class Branch
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? address { get; set; }
        public ICollection<Class>? classes { get; set; }
    }
}

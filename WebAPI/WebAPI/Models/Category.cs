namespace WebAPI.Models
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    /// <summary>
    /// Quan hệ danh mục
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Mã danh mục
        /// </summary>
        /// <example>DM001</example>
        public string? id { get; set; }
        /// <summary>
        /// Tên danh mục
        /// </summary>
        /// <example>Tin học văn phòng</example>
        public string? name { get; set; }
    }
}

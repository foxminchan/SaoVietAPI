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
    /// Quan hệ khoá học
    /// </summary>
    public class Course
    {
        /// <summary>
        /// Mã khoá học
        /// </summary>
        /// <example>TNVPOF0001</example>
        public string? id { get; set; }
        /// <summary>
        /// Tên khoá học
        /// </summary>
        /// <example>Tin học văn phòng</example>
        public string? name { get; set; }
        /// <summary>
        /// Mô tả khoá học
        /// </summary>
        /// <example>Tin học văn phòng Office 365</example>
        public string? description { get; set; }
        /// <summary>
        /// Mã danh mục
        /// </summary>
        /// <example>DM001</example>
        public string? categoryId { get; set; }
    }
}

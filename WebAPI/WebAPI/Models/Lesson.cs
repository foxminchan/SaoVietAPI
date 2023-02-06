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
    /// Quan hệ bài học
    /// </summary>
    public class Lesson
    {
        /// <summary>
        /// Mã bài học
        /// </summary>
        /// <example>THVPBH0001</example>
        public string? id { get; set; }
        /// <summary>
        /// Tên bài học
        /// </summary>
        /// <example>Giới thiệu về office 365</example>
        public string? name { get; set; }
        /// <summary>
        /// Mô tả bài học
        /// </summary>
        /// <example>Office 365 là một dịch vụ đám mây của Microsoft, cung cấp một loạt các ứng dụng văn phòng</example>
        public string? description { get; set; }
        /// <summary>
        /// Mã khoá học
        /// </summary>
        /// <example>TNVPOF0001</example>
        public string? courseId { get; set; }
    }
}

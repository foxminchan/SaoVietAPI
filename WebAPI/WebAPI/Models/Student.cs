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
    /// Quan hệ học viên
    /// </summary>
    public class Student
    {
        /// <summary>
        /// Họ tên học viên
        /// </summary>
        /// <example>Nguyễn Văn A</example>
        public string? fullName { get; set; }
        /// <summary>
        /// Ngày tháng năm sinh
        /// </summary>
        /// <example>2002-08-02</example>
        public string? dob { get; set; }
        /// <summary>
        /// Email học viên
        /// </summary>
        /// <example>example@email.com</example>
        public string? email { get; set; }
        /// <summary>
        /// Số điện thoại liên hệ
        /// </summary>
        /// <example>0123456789</example>
        public string? phone { get; set; }
    }
}

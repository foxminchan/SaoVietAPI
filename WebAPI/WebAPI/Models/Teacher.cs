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
    /// Quan hệ giáo viên
    /// </summary>
    public class Teacher
    {
        /// <summary>
        /// Họ và tên giáo viên
        /// </summary>
        /// <example>Nguyễn Đình Ánh</example>
        public string? fullName { get; set; }
        /// <summary>
        /// Email giáo viên
        /// </summary>
        /// <example>nd.anh@hutech.edu.vn</example>
        public string? email { get; set; }
        /// <summary>
        /// Số điện thoại giáo viên
        /// </summary>
        /// <example>0123456789</example>
        public string? phone { get; set; }
        /// <summary>
        /// Mã tài khoản giáo viên
        /// </summary>
        /// <example>GV001</example>
        public string? customerId { get; set; }
    }
}

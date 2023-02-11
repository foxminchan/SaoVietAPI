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
    /// Thông tin đăng nhập
    /// </summary>
    public class LoginUser
    {
        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        /// <example>example</example>
        public string? username { get; set; }
        /// <summary>
        /// Mật khẩu
        /// </summary>
        /// <example>password</example>
        public string? password { get; set; }
    }
}

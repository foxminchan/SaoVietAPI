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
    /// Chứng thực
    /// </summary>
    public class Auth
    {
        /// <summary>
        /// Token
        /// </summary>
        public string? token { get; set; }
        /// <summary>
        /// Refresh token
        /// </summary>
        public string? refreshToken { get; set; }
        /// <summary>
        /// Trạng thái chứng thực
        /// </summary>
        public bool isAuthSuccessful { get; set; }
        /// <summary>
        /// Danh sách lỗi
        /// </summary>
        public List<string>? errors { get; set; }
    }
}

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
    /// Thông tin refresh token
    /// </summary>
    public class TokenRefresh
    {
        /// <summary>
        /// Token
        /// </summary>
        public string? token { get; set; }
        /// <summary>
        /// Refresh token
        /// </summary>
        public string? refreshToken { get; set; }
    }
}

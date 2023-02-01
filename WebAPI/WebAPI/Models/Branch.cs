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
    /// Quản lý chi nhánh
    /// </summary>
    public class Branch
    {
        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        /// <example>TMBH0</example>
        public string? id { get; set; }
        /// <summary>
        /// Tên chi nhánh
        /// </summary>
        /// <example>Tân Mai Biên Hoà</example>
        public string? name { get; set; }
        /// <summary>
        /// Địa chỉ chi nhánh
        /// </summary>
        /// <example>Số 46B/3, KP 2, Phường Tân Mai, Thành Phố Biên Hòa</example>
        public string? address { get; set; }
    }
}

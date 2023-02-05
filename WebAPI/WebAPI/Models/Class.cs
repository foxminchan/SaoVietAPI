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
    /// Quan hệ lớp học
    /// </summary>
    public class Class
    {
        /// <summary>
        /// Mã lớp học
        /// </summary>
        /// <example>DOHOA01</example>
        public string? id { get; set; }
        /// <summary>
        /// Tên lớp học
        /// </summary>
        /// <example>AutoCad và Adobe illustrator 01</example>
        public string? name { get; set; }
        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        /// <example>2023-01-01</example>
        public string? startDate { get; set; }
        /// <summary>
        /// Ngày kết thúc
        /// </summary>
        /// <example>2023-02-01</example>
        public string? endDate { get; set; }
        /// <summary>
        /// Mã giáo viên phụ trách
        /// </summary>
        /// <example>d9f9b9f0-5b5a-4b9f-9b9f-0d5b5a4b9f9b</example>
        public Guid? teacherId { get; set; }
        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        /// <example>CN001</example>
        public string? branchId { get; set; }
    }
}

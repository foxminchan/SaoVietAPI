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
    /// Quan hệ sổ điểm danh
    /// </summary>
    public class Attendance
    {
        /// <summary>
        /// Mã lớp
        /// </summary>
        /// <example>DOHOA01</example>
        public string? classId { get; set; }
        /// <summary>
        /// Mã bài học
        /// </summary>
        /// <example>THVPBH0001</example>
        public string? lessonId { get; set; }
        /// <summary>
        /// Ngày điểm danh
        /// </summary>
        /// <example>2023-01-15</example>
        public string? date { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        /// <example>Lớp học đầy đủ</example>
        public string? comment { get; set; }
        /// <summary>
        /// Đánh giá
        /// </summary>
        /// <example>10</example>
        public byte? evaluation { get; set; }
        /// <summary>
        /// Sĩ số có mặt
        /// </summary>
        /// <example>30</example>
        public byte? attendance { get; set; }
    }
}

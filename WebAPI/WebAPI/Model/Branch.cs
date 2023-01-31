namespace WebAPI.Model
{
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

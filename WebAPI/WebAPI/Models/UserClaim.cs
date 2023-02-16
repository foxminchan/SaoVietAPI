namespace WebAPI.Models
{
    /// <summary>
    /// Quan hệ quyền truy cập
    /// </summary>
    public class UserClaim
    {
        /// <summary>
        /// Mã người dùng
        /// </summary>
        public string? userId { get; set; }
        /// <summary>
        /// Loại quyền
        /// </summary>
        public string? claimType { get; set; }
        /// <summary>
        /// Tên quyền
        /// </summary>
        public string? claimValue { get; set; }
    }
}

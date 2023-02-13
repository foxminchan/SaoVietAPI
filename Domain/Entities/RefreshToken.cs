namespace Domain.Entities
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public class RefreshToken
    {
        public int id { get; set; }
        public string? userId { get; set; }
        public string? token { get; set; }
        public string? jwtId { get; set; }
        public bool isUsed { get; set; }
        public bool isRevoked { get; set; }
        public DateTime? addedDate { get; set; }
        public DateTime? expiryDate { get; set; }
    }
}

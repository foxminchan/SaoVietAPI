using Domain.Entities;

namespace Domain.Interfaces
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public interface IUserRepository
    {
        public Task Register(ApplicationUser user);
        public Task<bool> CheckAccountValid(string username, string password);
        public Task<bool> CheckTwoFa(ApplicationUser user);
        public Task<bool> CheckEmailConfirmed(ApplicationUser user);
        public Task<bool> CheckPhoneConfirmed(ApplicationUser user);
        public Task<bool> IsUserNameExists(string username);
    }
}

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

    public interface IApplicationUserRepository
    {
        public void Register(ApplicationUser user);
        public void FailLogin(string username);
        public void LockAccount(string username);
        public void ResetFailLogin(string username);
        public void BanAccount(string username);
        public bool CheckAccountValid(string username, string password);
        public bool IsLockedAccount(string username);
        public bool IsUserNameExists(string username);
        public bool IsHasUserById(string userId);
        public int GetFailLogin(string username);
        public string GetUserId(string username);
        public ApplicationUser GetById(string userId);
        public ApplicationUser GetByUserName(string username);
    }
}

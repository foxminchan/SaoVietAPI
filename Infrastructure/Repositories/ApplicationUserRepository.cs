using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository
    {
        public ApplicationUserRepository(ApplicationDbContext context, ICache cache) : base(context, cache)
        {
        }

        public void Register(ApplicationUser user) => Insert(user);

        public bool CheckAccountValid(string username, string password) => Any(x => x.UserName == username) && BCrypt.Net.BCrypt.Verify(password, GetList(x => x.UserName == username && x.PasswordHash != null).Select(x => x.PasswordHash).SingleOrDefault());

        public bool IsUserNameExists(string username) => Any(x => x.UserName == username);

        public bool IsLockedAccount(string username) => Any(x => x.UserName == username && x.LockoutEnabled && x.LockoutEnd > DateTime.Now);

        public int GetFailLogin(string username) => GetMany(x => x.UserName == username).First().AccessFailedCount;

        public ApplicationUser GetById(string userId) => GetMany(x => x.Id == userId).First();

        public ApplicationUser GetByUserName(string username) => GetMany(x => x.UserName == username).First();

        public void FailLogin(string username)
        {
            var user = GetList(x => x.UserName == username).First();
            user.AccessFailedCount++;
            Update(user);
        }

        public void LockAccount(string username)
        {
            var user = GetList(x => x.UserName == username).First();
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTime.Now.AddMinutes(30);
            Update(user);
        }

        public void ResetFailLogin(string username)
        {
            var user = GetList(x => x.UserName == username).First();
            user.AccessFailedCount = 0;
            Update(user);
        }

        public void BanAccount(string username)
        {
            var user = GetList(x => x.UserName == username).First();
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTime.Now.AddYears(200);
            Update(user);
        }

        public string GetUserId(string username) => GetList(x => x.UserName == username).First().Id;

        public bool IsHasUserById(string userId) => Any(x => x.Id == userId);
    }
}

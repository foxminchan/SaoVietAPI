﻿using Domain.Entities;
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

        public async Task Register(ApplicationUser user) => await Insert(user);

        public async Task<bool> CheckAccountValid(string username, string password)
        {
            if (!await Any(x => x.UserName == username)) return false;
            var user = await GetList(x => x.UserName == username && x.PasswordHash != null);
            return BCrypt.Net.BCrypt.Verify(password,  user.Select(x => x.PasswordHash).SingleOrDefault());
        }

        public async Task<bool> CheckTwoFa(ApplicationUser user) =>
            await Any(x => x.Id == user.Id && x.TwoFactorEnabled);

        public async Task<bool> CheckEmailConfirmed(ApplicationUser user) =>
            await Any(x => x.Id == user.Id && x.EmailConfirmed);

        public async Task<bool> CheckPhoneConfirmed(ApplicationUser user) =>
            await Any(x => x.Id == user.Id && x.PhoneNumberConfirmed);

        public async Task<bool> IsUserNameExists (string username) =>
            await Any(x => x.UserName == username);

        public async Task<bool> IsLockedAccount(string username) =>
            await Any(x => x.UserName == username && x.LockoutEnabled && x.LockoutEnd > DateTime.Now);

        public async Task<int> GetFailLogin(string username)
        {
            var userList = await GetList(x => x.UserName == username);
            var user = userList.First();
            return user.AccessFailedCount;
        }

        public async Task<ApplicationUser> GetById(string id)
        {
            var result = await GetList(x => x.Id == id);
            return result.First();
        }

        public async Task<ApplicationUser> GetByUserName(string username)
        {
            var userList = await GetList(x => x.UserName == username);
            return userList.First();
        }

        public async Task FailLogin(string username)
        {
            var userList = await GetList(x => x.UserName == username);
            var user = userList.First();
            user.AccessFailedCount++;
            await Update(user);
        }

        public async Task LockAccount(string username)
        {
            var userList = await GetList(x => x.UserName == username);
            var user = userList.First();
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTime.Now.AddMinutes(30);
            await Update(user);
        }

        public async Task UnlockAccount(string username)
        {
            var userList = await GetList(x => x.UserName == username);
            var user = userList.First();
            user.LockoutEnabled = false;
            user.LockoutEnd = null;
            await Update(user);
        }

        public async Task ResetFailLogin(string username)
        {
            var userList = await GetList(x => x.UserName == username);
            var user = userList.First();
            user.AccessFailedCount = 0;
            await Update(user);
        }

        public async Task BanAccount(string username)
        {
            var userList = await GetList(x => x.UserName == username);
            var user = userList.First();
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTime.Now.AddYears(200);
            await Update(user);
        }

        public async Task<string> GetUserId(string username)
        {
            var userList = await GetList(x => x.UserName == username);
            var user = userList.First();
            return user.Id;
        }
    }
}

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

    public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
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
            await Any(x => x.Id == user.Id && x.TwoFactorEnabled == true);

        public async Task<bool> CheckEmailConfirmed(ApplicationUser user) =>
            await Any(x => x.Id == user.Id && x.EmailConfirmed == true);

        public async Task<bool> CheckPhoneConfirmed(ApplicationUser user) =>
            await Any(x => x.Id == user.Id && x.PhoneNumberConfirmed == true);

        public async Task<bool> IsUserNameExists (string username) =>
            await Any(x => x.UserName == username);
    }
}

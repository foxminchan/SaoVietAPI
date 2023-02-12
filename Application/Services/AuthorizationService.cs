using Application.Cache;
using Application.Common;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Repositories;

namespace Application.Services
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public class AuthorizationService : BaseService
    {
        private readonly IApplicationUserRepository _userRepository;
        public AuthorizationService(ApplicationDbContext context, ICache cache) : base(context, cache) => _userRepository = new ApplicationUserRepository(context, cache);

        public async Task<bool> CheckAccountValid(string username, string password) =>
            await _userRepository.CheckAccountValid(username, password);

        public async Task<bool> CheckTwoFa(ApplicationUser user) => await _userRepository.CheckTwoFa(user);

        public async Task<bool> CheckEmailConfirmed(ApplicationUser user) =>
            await _userRepository.CheckEmailConfirmed(user);

        public async Task<bool> CheckPhoneConfirmed(ApplicationUser user) => await _userRepository.CheckPhoneConfirmed(user);

        public async Task<bool> CheckUserExist(string username) => await _userRepository.IsUserNameExists(username);

        public async Task Register(ApplicationUser user)
        {
            await _userRepository.Register(user);
            await SaveAsync();
        }

        public async Task<bool> IsLockedAccount(string username) => await _userRepository.IsLockedAccount(username);

        public async Task FailLogin(string username)
        {
            await _userRepository.FailLogin(username);
            await SaveAsync();
            var failLogin = await _userRepository.GetFailLogin(username);
            if (failLogin >= 5)
            {
                await _userRepository.LockAccount(username);
                await SaveAsync();
            }
            if (failLogin >= 10)
            {
                await _userRepository.BanAccount(username);
                await SaveAsync();
            }
        }

        public async Task UnlockAccount(string username)
        {
            await _userRepository.UnlockAccount(username);
            await SaveAsync();
        }

        public async Task ResetFailLogin(string username)
        {
            await _userRepository.ResetFailLogin(username);
            await SaveAsync();
        }
    }
}

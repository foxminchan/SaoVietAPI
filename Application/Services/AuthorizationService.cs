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
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthorizationService(ApplicationDbContext context, ICache cache) : base(context)
        {
            _refreshTokenRepository = new RefreshTokenRepository(context, cache);
            _userRepository = new ApplicationUserRepository(context, cache);
        } 

        public bool CheckAccountValid(string username, string password) => _userRepository.CheckAccountValid(username, password);

        public bool CheckUserExist(string username) => _userRepository.IsUserNameExists(username);

        public string GetUserId(string username) => _userRepository.GetUserId(username);

        public bool IsLockedAccount(string username) => _userRepository.IsLockedAccount(username);

        public RefreshToken GetToken(string token) => _refreshTokenRepository.GetToken(token);

        public ApplicationUser GetUserById(string userId) => _userRepository.GetById(userId);

        public ApplicationUser GetUserByUserName(string username) => _userRepository.GetByUserName(username);

        public void Register(ApplicationUser user)
        {
            _userRepository.Register(user);
            Save();
        }

        public void FailLogin(string username)
        {
            _userRepository.FailLogin(username);
            Save();
            var failLogin = _userRepository.GetFailLogin(username);
            if (failLogin >= 5)
            {
                _userRepository.LockAccount(username);
                Save();
            }

            if (failLogin < 10) return;
            _userRepository.BanAccount(username);
            Save();
        }

        public void ResetFailLogin(string username)
        {
            _userRepository.ResetFailLogin(username);
            Save();
        }

        public void AddToken(RefreshToken token)
        {
            _refreshTokenRepository.AddToken(token);
            Save();
        }

        public void UpdateRefreshToken(RefreshToken token)
        {
            _refreshTokenRepository.UpdateToken(token);
            Save();
        }
    }
}

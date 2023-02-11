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
        private readonly IUserRepository _userRepository;
        public AuthorizationService(ApplicationDbContext context) : base(context) => _userRepository = new UserRepository(context);

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
    }
}

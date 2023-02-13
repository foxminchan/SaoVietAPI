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

    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context, ICache memoryCache) : base(context, memoryCache)
        {
        }

        public async Task AddToken(RefreshToken token) => await Insert(token);

        public async Task<RefreshToken> GetToken(string token)
        {
            var result = await GetList(filter: x => x.token == token);
            return result.First();
        }

        public async Task UpdateToken(RefreshToken token) => await Update(token);
    }
}

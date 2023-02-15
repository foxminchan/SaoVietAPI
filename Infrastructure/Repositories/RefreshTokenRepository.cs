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
        public RefreshTokenRepository(ApplicationDbContext context, ICache cache) : base(context, cache)
        {
        }

        public void AddToken(RefreshToken token) => Insert(token);

        public RefreshToken GetToken(string token) => GetList(filter: x => x.token == token).First();

        public void UpdateToken(RefreshToken token) => Update(token);
    }
}

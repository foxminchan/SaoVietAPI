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

    public class IdentityClaimRepository : GenericRepository<IdentityClaim>, IIdentityClaimRepository
    {
        public IdentityClaimRepository(ApplicationDbContext context, ICache cache) : base(context, cache)
        {
        }

        public bool IsIdentityClaimExist(string userid, string claimType, string claimValue) =>
            Any(x => x.UserId == userid && x.ClaimType == claimType && x.ClaimValue == claimValue);

        public void AddIdentityClaim(IdentityClaim identityClaim) => Insert(identityClaim);

        public void DeleteIdentityClaim(string userId, string claimType, string claimValue) => Delete(x => x.UserId == userId && x.ClaimType == claimType && x.ClaimValue == claimValue);

        public void UpdateIdentityClaim(IdentityClaim identityClaim) => Update(identityClaim);

        public IEnumerable<string?> GetClaimTypeByUserId(string userId) => GetMany(x => x.UserId == userId).Select(x => x.ClaimType).Distinct();
    }
}

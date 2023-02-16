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

    public interface IIdentityClaimRepository : IGenericRepository<IdentityClaim>
    {
        public IEnumerable<string?> GetClaimTypeByUserId(string userId);
        public bool IsIdentityClaimExist(string userid,string claimType, string claimValue);
        public void AddIdentityClaim(IdentityClaim identityClaim);
        public void DeleteIdentityClaim(string userId, string claimType, string claimValue);
        public void UpdateIdentityClaim(IdentityClaim identityClaim);
    }
}

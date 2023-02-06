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

    public interface IBranchRepository : IGenericRepository<Branch>
    {
        public Task<List<Branch>> GetBranches();
        public Task<List<Branch>> GetBranchesByNames(string? name);
        public Task<List<Branch>> GetBranchesByZone(string? zone);
        public Task<Branch?> GetBranchById(string? id);
        public Task AddBranch(Branch branch);
        public Task UpdateBranch(Branch branch, string id);
        public Task DeleteBranch(string id);
    }
}

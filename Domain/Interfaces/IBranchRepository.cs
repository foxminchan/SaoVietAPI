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
        public List<Branch> GetBranches();
        public List<Branch> GetBranchesByNames(string? name);
        public List<Branch> GetBranchesByZone(string? zone);
        public Branch? GetBranchById(string? id);
        public void AddBranch(Branch branch);
        public void UpdateBranch(Branch branch, string id);
        public void DeleteBranch(string id);
    }
}

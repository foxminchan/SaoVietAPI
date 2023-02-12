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

    public class BranchRepository : GenericRepository<Branch>, IBranchRepository
    {
        public BranchRepository(ApplicationDbContext context, ICache cache) : base(context, cache)
        {
        }

        public async Task<List<Branch>> GetBranches()
        {
            var branches = await GetAll();
            return branches.ToList();
        }

        public async Task<List<Branch>> GetBranchesByNames(string? name)
        {
            var branches = await GetList(filter: x => name != null && x.name != null && x.name.Contains(name));
            return branches.ToList();
        }

        public async Task<List<Branch>> GetBranchesByZone(string? zone)
        {
            var branches = await GetList(filter: x => x.address != null && zone != null && x.address.Contains(zone));
            return branches.ToList();
        }

        public async Task<Branch?> GetBranchById(string? id) => id == null ? null : await GetById(id);

        public async Task AddBranch(Branch branch) => await Insert(branch);

        public async Task UpdateBranch(Branch branch, string id) => await Update(branch, x => x.id == id);

        public async Task DeleteBranch(string id) => await Delete(x => x.id == id);
    }
}

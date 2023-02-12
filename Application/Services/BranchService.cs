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
    
    public class BranchService : BaseService
    {
        private readonly IBranchRepository _branchRepository;

        public BranchService(ApplicationDbContext context, ICache cache) : base(context, cache) => _branchRepository = new BranchRepository(context, cache);

        public async Task<List<Branch>> GetBranches() => await _branchRepository.GetBranches();

        public async Task<List<Branch>> GetBranchesByNames(string? name) => await _branchRepository.GetBranchesByNames(name);

        public async Task<List<Branch>> GetBranchesByZone(string? zone) => await _branchRepository.GetBranchesByZone(zone);

        public async Task<Branch?> GetBranchById(string? id) => await _branchRepository.GetBranchById(id);

        public async Task AddBranch(Branch branch)
        {
            await _branchRepository.AddBranch(branch);
            await SaveAsync();
        }

        public async Task UpdateBranch(Branch branch, string id)
        {
            await _branchRepository.UpdateBranch(branch, id);
            await SaveAsync();
        }

        public async Task DeleteBranch(string id)
        {
            await _branchRepository.DeleteBranch(id);
            await SaveAsync();
        }
    }   
}

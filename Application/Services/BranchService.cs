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

        public BranchService(ApplicationDbContext context, ICache cache) : base(context) => _branchRepository = new BranchRepository(context, cache);

        public IEnumerable<Branch> GetBranches() => _branchRepository.GetBranches();

        public IEnumerable<Branch> GetBranchesByNames(string? name) => _branchRepository.GetBranchesByNames(name);

        public IEnumerable<Branch> GetBranchesByZone(string? zone) => _branchRepository.GetBranchesByZone(zone);

        public Branch? GetBranchById(string? id) => _branchRepository.GetBranchById(id);

        public void AddBranch(Branch branch)
        {
            _branchRepository.AddBranch(branch);
            Save();
        }

        public void UpdateBranch(Branch branch)
        {
            _branchRepository.UpdateBranch(branch);
            Save();
        }

        public void DeleteBranch(string id)
        {
            _branchRepository.DeleteBranch(id);
            Save();
        }
    }   
}

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

        public BranchService(ApplicationDbContext context) : base(context)
        {
            _branchRepository = new BranchRepository(context);
        }

        public List<Branch> GetBranches() => _branchRepository.GetBranches();

        public List<Branch> GetBranchesByNames(string? name) => _branchRepository.GetBranchesByNames(name);

        public Branch? GetBranchById(string? id) => _branchRepository.GetBranchById(id);

        public Task AddBranch(Branch branch)
        {
            _branchRepository.AddBranch(branch);
            return SaveAsync();
        }

        public Task UpdateBranch(Branch branch, string id)
        {
            _branchRepository.UpdateBranch(branch, id);
            return SaveAsync();
        }

        public Task DeleteBranch(string id)
        {
            _branchRepository.DeleteBranch(id);
            return SaveAsync();
        }
    }   
}

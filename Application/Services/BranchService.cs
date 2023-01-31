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
        private readonly ApplicationDbContext _context;
        private readonly IBranchRepository _branchRepository;

        public BranchService(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _branchRepository = new BranchRepository(context);
        }

        public Branch? GetBranchById(string? id) => _branchRepository.GetBranchById(id);
    }   
}

using Application.Cache;
using Domain.Interfaces;
using Infrastructure;

namespace Application.Common
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public class BaseService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICache _cache;

        protected BaseService(ApplicationDbContext context, ICache cache)
        {
            _context = context;
            _cache = cache;
        } 

        protected async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}

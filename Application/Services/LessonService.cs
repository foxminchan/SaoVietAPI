using Domain.Interfaces;
using Infrastructure.Repositories;
using Infrastructure;
using Application.Common;

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
    
    public class LessonService : BaseService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILessonRepository _lessonRepository;

        public LessonService(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _lessonRepository = new LessonRepository(context);
        }
    }
}

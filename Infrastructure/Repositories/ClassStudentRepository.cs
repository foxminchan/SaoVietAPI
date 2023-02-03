﻿using Domain.Entities;
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

    public class ClassStudentRepository : GenericRepository<ClassStudent>, IClassStudentRepository
    {
        public ClassStudentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public int CountStudentInClass(string? classId) => Count(x => x.classId == classId);

        public int CountClassByStudent(Guid? studentId) => Count(x => x.studentId == studentId);

        public List<Guid?> GetAllStudentIdByClassId(string? classId) => GetList(x => x.classId == classId).Select(x => x.studentId).ToList();

        public List<string?> GetAllClassIdByStudentId(Guid? studentId) => GetList(x => x.studentId == studentId).Select(x => x.classId).ToList();
    }
}

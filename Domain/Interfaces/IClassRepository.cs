﻿using Domain.Entities;

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

    public interface IClassRepository : IGenericRepository<Class>
    {
        public List<Class> GetClasses();
        public List<Class> FindClassByName(string? name);
        public List<Class> GetClassesByStatus(string? status);
        public void AddClass(Class entity);
        public void UpdateClass(Class entity, string id);
        public void DeleteClass(string id);
    }
}

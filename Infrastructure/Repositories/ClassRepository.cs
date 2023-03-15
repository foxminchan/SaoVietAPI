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

    public class ClassRepository : GenericRepository<Class>, IClassRepository
    {
        public ClassRepository(ApplicationDbContext context, ICache cache) : base(context, cache)
        {
        }

        public IEnumerable<Class> GetClasses() => GetAll();

        public IEnumerable<Class> FindClassByName(string? name) => GetMany(x => x.name != null && name != null && x.name.Contains(name));

        public IEnumerable<Class> GetClassesByStatus(string? status)
        {
            if (status == null) return GetAll();
            if (status.Contains("Expired"))
                return GetMany(x => Convert.ToDateTime(x.endDate) < DateTime.Now);
            if (status.Contains("Active"))
                return GetMany(x => Convert.ToDateTime(x.startDate) <= DateTime.Now && Convert.ToDateTime(x.endDate) >= DateTime.Now);
            return status.Contains("Upcoming") ? GetMany(x => Convert.ToDateTime(x.startDate) > DateTime.Now) : GetAll();
        }

        public IEnumerable<Class> FindClassByTeacher(Guid? teacherId) => GetMany(x => teacherId != null && x.teacherId == teacherId);

        public Class? FindClassById(string? id) => GetById(id);

        public void AddClass(Class entity) => Insert(entity);

        public void UpdateClass(Class entity) => Update(entity);

        public void DeleteClass(string id) => Delete(x => x.id == id);

        public bool ClassExists(string? id) => Any(x => x.id == id);
    }
}

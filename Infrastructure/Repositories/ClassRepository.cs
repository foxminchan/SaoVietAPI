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
        public ClassRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Class>> GetClasses()
        {
            var classes = await GetAll();
            return classes.ToList();
        }

        public async Task<List<Class>> FindClassByName(string? name)
        {
            var classes = await GetList(filter: x => x.name != null && name != null && x.name.Contains(name));
            return classes.ToList();
        }

        public async Task<List<Class>> GetClassesByStatus(string? status)
        {
            var classes = await GetClasses();
            if (status == null) return classes;
            if (status.Contains("Expired"))
                return classes.Where(x => Convert.ToDateTime(x.endDate) < DateTime.Now).ToList();
            if (status.Contains("Active"))
                return classes.Where(x => Convert.ToDateTime(x.startDate) <= DateTime.Now && Convert.ToDateTime(x.endDate) >= DateTime.Now).ToList();
            return status.Contains("Upcoming") ? classes.Where(x => Convert.ToDateTime(x.startDate) > DateTime.Now).ToList() : classes;
        }

        public async Task<List<Class>> FindClassByTeacher(Guid? teacherId)
        {
            var classes = await GetList(filter: x => teacherId != null && x.teacherId == teacherId);
            return classes.ToList();
        }

        public async Task<Class?> FindClassById(string? id) => await GetById(id);

        public async Task AddClass(Class newClass) => await Insert(newClass);

        public async Task UpdateClass(Class newClass, string id) => await Update(newClass, x => x.id == id);

        public async Task DeleteClass(string id) => await Delete(x => x.id == id);

        public async Task<bool> ClassExists(string? id) => await Any(x => x.id == id);
    }
}

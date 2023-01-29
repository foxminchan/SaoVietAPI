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

        public List<Class> GetClasses() => GetAll().ToList();

        public void AddClass(Class newClass) => Insert(newClass);

        public void UpdateClass(Class newClass, string id) => Update(newClass, x => x.id == id);

        public void DeleteClass(string id) => Delete(x => x.id == id);

        public List<Class> FindClassByName(string name) => GetList(filter: x => x.name != null && x.name.Contains(name)).ToList();

        public List<Class> GetClassesByStatus(string? status)
        {
            var classes = new List<Class>();
            if (status == null) return GetClasses();
            if (status.Contains("Expired"))
                classes = GetList(filter: x => x.endDate != null && DateTime.Parse(x.endDate) < DateTime.Now).ToList();
            else if (status.Contains("Active"))
                classes = GetList(filter: x => x.endDate != null && DateTime.Parse(x.endDate) >= DateTime.Now).ToList();
            else if (status.Contains("Upcoming"))
                classes = GetList(filter: x => x.startDate != null && DateTime.Parse(x.startDate) > DateTime.Now).ToList();
            return classes;
        }

    }

}

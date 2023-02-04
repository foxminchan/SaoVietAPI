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

        public List<Class> FindClassByName(string? name) => GetList(filter: x => name != null && x.name != null && x.name.Contains(name)).ToList();

        public List<Class> GetClassesByStatus(string? status)
        {
            var classes = new List<Class>();
            if (status == null) return classes;
            if (status.Contains("Expired"))
                classes = GetList(filter: x => x.endDate != null).Where(x => Convert.ToDateTime(x.endDate) < DateTime.Now).AsEnumerable().ToList();
            else if (status.Contains("Active"))
                classes = GetList(filter: x => x.endDate != null).Where(x => Convert.ToDateTime(x.endDate) >= DateTime.Now).AsEnumerable().ToList();
            else if (status.Contains("Upcoming"))
                classes = GetList(filter: x => x.startDate != null).Where(x => Convert.ToDateTime(x.startDate) > DateTime.Now).AsEnumerable().ToList();
            return classes;
        }

        public List<Class> FindClassByTeacher(Guid? teacherId) => GetList(filter: x => teacherId != null && x.teacherId == teacherId).ToList();

        public Class? FindClassById(string? id) => GetById(id);

        public void AddClass(Class newClass) => Insert(newClass);

        public void UpdateClass(Class newClass, string id) => Update(newClass, x => x.id == id);

        public void DeleteClass(string id) => Delete(x => x.id == id);

        public bool ClassExists(string? id) => Any(x => x.id == id);
    }
}

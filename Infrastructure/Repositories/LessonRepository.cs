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

    public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
    {
        public LessonRepository(ApplicationDbContext context, ICache cache) : base(context, cache)
        {
        }

        public IEnumerable<Lesson> GetAllLesson() => GetAll();

        public IEnumerable<Lesson> GetByNames(string? name) =>
            GetMany(x => name != null && x.name != null && x.name.Contains(name));

        public Lesson? GetLessonById(string id) => GetById(id);

        public void AddLesson(Lesson lesson) => Insert(lesson);

        public void UpdateLesson(Lesson lesson) => Update(lesson);

        public void DeleteLesson(string lesson) => Delete(x => x.id == lesson);

        public bool LessonExists(string id) => Any(x => x.id == id);
    }
}

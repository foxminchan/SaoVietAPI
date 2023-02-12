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

        public async Task<List<Lesson>> GetAllLesson()
        {
            var lessons = await GetAll();
            return lessons.ToList();
        }

        public async Task<List<Lesson>> GetByNames(string? name)
        {
            var lessons = await GetList(x => name != null && x.name != null && x.name.Contains(name));
            return lessons.ToList();
        }

        public async Task<Lesson?> GetLessonById(string id) => await GetById(id);

        public async Task AddLesson(Lesson lesson) => await Insert(lesson);

        public async Task UpdateLesson(Lesson lesson, string? id) => await Update(lesson, x => x.id == id);

        public async Task DeleteLesson(Lesson lesson) => await Delete(lesson);

        public async Task<bool> LessonExists(string id) => await Any(x => x.id == id);
    }
}

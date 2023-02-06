using Domain.Entities;

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

    public interface ILessonRepository : IGenericRepository<Lesson>
    {
        public Task<List<Lesson>> GetAllLesson();
        public Task<List<Lesson>> GetByNames(string? name);
        public Task<Lesson?> GetLessonById(string id);
        public Task AddLesson(Lesson lesson);
        public Task UpdateLesson(Lesson lesson, string? id);
        public Task DeleteLesson(Lesson lesson);
        public Task<bool> LessonExists(string id);
    }
}

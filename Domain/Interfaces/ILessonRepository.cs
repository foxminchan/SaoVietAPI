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
        public IEnumerable<Lesson> GetAllLesson();
        public IEnumerable<Lesson> GetByNames(string? name);
        public Lesson? GetLessonById(string id);
        public void AddLesson(Lesson lesson);
        public void UpdateLesson(Lesson lesson);
        public void DeleteLesson(string lesson);
        public bool LessonExists(string id);
    }
}

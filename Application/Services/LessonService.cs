using Domain.Entities;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Repositories;

namespace Application.Services
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public class LessonService
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ICourseRepository _courseRepository;

        public LessonService(ApplicationDbContext context, ICache cache)
        {
            _lessonRepository = new LessonRepository(context, cache);
            _courseRepository = new CourseRepository(context, cache);
        }

        public IEnumerable<Lesson> GetAllLesson() => _lessonRepository.GetAllLesson();

        public IEnumerable<Lesson> GetByNames(string? name) => _lessonRepository.GetByNames(name);

        public Lesson? GetLessonById(string id) => _lessonRepository.GetLessonById(id);

        public void AddLesson(Lesson lesson) => _lessonRepository.AddLesson(lesson);

        public void UpdateLesson(Lesson lesson) => _lessonRepository.UpdateLesson(lesson);

        public void DeleteLesson(string id) => _lessonRepository.DeleteLesson(id);

        public bool IsCourseExists(string id) => _courseRepository.CourseExists(id);
    }
}

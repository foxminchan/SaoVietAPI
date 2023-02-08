using Domain.Interfaces;
using Infrastructure.Repositories;
using Infrastructure;
using Application.Common;
using Domain.Entities;

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
    
    public class LessonService : BaseService
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ICourseRepository _courseRepository;

        public LessonService(ApplicationDbContext context) : base(context)
        {
            _lessonRepository = new LessonRepository(context);
            _courseRepository = new CourseRepository(context);
        }

        public async Task<List<Lesson>> GetAllLesson() => await _lessonRepository.GetAllLesson();

        public async Task<List<Lesson>> GetByNames(string? name) => await _lessonRepository.GetByNames(name);

        public async Task<Lesson?> GetLessonById(string id) => await _lessonRepository.GetLessonById(id);

        public async Task AddLesson(Lesson lesson)
        {
            await _lessonRepository.AddLesson(lesson);
            await SaveAsync();
        }

        public async Task UpdateLesson(Lesson lesson, string? id)
        {
            await _lessonRepository.UpdateLesson(lesson, id);
            await SaveAsync();
        }

        public async Task DeleteLesson(Lesson lesson)
        {
            await _lessonRepository.DeleteLesson(lesson);
            await SaveAsync();
        }

        public async Task<bool> IsCourseExists(string id) => await _courseRepository.CourseExists(id);
    }
}

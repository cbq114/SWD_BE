using Microsoft.AspNetCore.Http;
using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.LessonModel;

namespace Tutor.Applications.Services
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository;

        public LessonService(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        public async Task<string> AddLesson(CreateLessonModel model, string username, IFormFile file)
        {
            return await _lessonRepository.AddLesson(model, username, file);
        }

        public Task<string> DeleteLesson(int lessonId)
        {
            return _lessonRepository.DeleteLesson(lessonId);
        }

        public Task<LessonViewModel> GetLessonById(int lessonId)
        {
            return _lessonRepository.GetLessonById(lessonId);
        }

        public Task<List<LessonViewModel>> GetLessonByTutor(string username)
        {
            return _lessonRepository.GetLessonByTutor(username);
        }

        public Task<string> UpdateLesson(CreateLessonModel model, string username, IFormFile imageUrl, int lessonId)
        {
            return _lessonRepository.UpdateLesson(model, username, imageUrl, lessonId);
        }
    }
}

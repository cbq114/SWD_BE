using Microsoft.AspNetCore.Http;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Models.LessonModel;

namespace Tutor.Infratructures.Interfaces
{
    public interface ILessonRepository
    {
        Task<string> AddLesson(CreateLessonModel model, string username, IFormFile imageUrl);
        Task<string> UpdateLesson(CreateLessonModel model, string username, IFormFile imageUrl, int lessonId);
        Task<string> DeleteLesson(int lessonId);
        Task<LessonViewModel> GetLessonById(int lessonId);
        Task<List<LessonViewModel>> GetLessonByTutor(string username);
        Task<Lessons> GetLessonsById(int lessonId);


    }
}

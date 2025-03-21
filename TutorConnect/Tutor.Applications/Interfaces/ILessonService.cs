using Microsoft.AspNetCore.Http;
using Tutor.Infratructures.Models.LessonModel;

namespace Tutor.Applications.Interfaces
{
    public interface ILessonService
    {
        Task<string> AddLesson(CreateLessonModel model, string username, IFormFile formFile);
        Task<string> UpdateLesson(CreateLessonModel model, string username, IFormFile imageUrl, int lessonId);
        Task<string> DeleteLesson(int lessonId);
        Task<LessonViewModel> GetLessonById(int lessonId);
        Task<List<LessonViewModel>> GetLessonByTutor(string username);
    }
}

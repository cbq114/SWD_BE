using Tutor.Domains.Enums;
using Tutor.Infratructures.Models.LessonModel;

namespace Tutor.Infratructures.Models.HomeInstructorModel
{
    public class InstructorDetailsModelHome
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Avatar { get; set; }
        public string Address { get; set; }
        public string TeachingExperience { get; set; }
        public string Education { get; set; }
        public string Country { get; set; }

        public int LanguageId { get; set; }
        public decimal Price { get; set; }
        public int TotalLessons { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public int TotalFeedbacks { get; set; }
        public ICollection<LessonViewModel> Lessons { get; set; }
    }

    // Model chứa chi tiết bài học của giảng viên
    public class LessonViewModelHome
    {
        public string Title { get; set; }
        public LevelLessonEnum Level { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreateAt { get; set; }
        public LessonStatus Status { get; set; }
    }
    public class InstructorRatingSummary
    {
        public double AverageRating { get; set; }
        public int TotalFeedbacks { get; set; }
    }
}

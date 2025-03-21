using Tutor.Domains.Enums;

namespace Tutor.Infratructures.Models.LessonModel
{
    public class LessonViewModel
    {
        public int LessonId { get; set; }
        public string Instructor { get; set; }
        public LevelLessonEnum Level { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public string ImageUrl { get; set; }
        public string LearningObjectives { get; set; }
        public DateTime CreateAt { get; set; }
        public LessonStatus Status { get; set; }
        public string Material { get; set; }
        public string LanguageName { get; set; }
        // Thông tin bài học cha
        public int? ParentLessonId { get; set; }

        public string? ParentLessonTitle { get; set; }
    }
}

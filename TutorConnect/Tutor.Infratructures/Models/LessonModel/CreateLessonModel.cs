using Tutor.Domains.Enums;

namespace Tutor.Infratructures.Models.LessonModel
{
    public class CreateLessonModel
    {
        public LevelLessonEnum Level { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string LearningObjectives { get; set; }
        public LessonStatus Status { get; set; }
        public string Material { get; set; }

        // Bài học cha (có thể null)
        public int? ParentLessonId { get; set; }
    }
}

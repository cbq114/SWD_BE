using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tutor.Domains.Enums;
using Tutor.Shared.Helper;

namespace Tutor.Domains.Entities
{
    public class Lessons
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LessonId { get; set; }
        public string Instructor { get; set; }
        public LevelLessonEnum Level { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public string LearningObjectives { get; set; }
        public DateTime CreateAt { get; set; } = DateTimeHelper.GetVietnamNow();
        public LessonStatus Status { get; set; }
        public string Material { get; set; }

        [ForeignKey("Instructor")]
        public Users User { get; set; }
        public virtual ICollection<Bookings> Bookings { get; set; }
        public virtual ICollection<Promotions> Promotions { get; set; }
        public int? ParentLessonId { get; set; }

        [ForeignKey("ParentLessonId")]
        public virtual Lessons? ParentLesson { get; set; }

        public virtual ICollection<Lessons> PrerequisiteLessons { get; set; } = new List<Lessons>();
    }

}

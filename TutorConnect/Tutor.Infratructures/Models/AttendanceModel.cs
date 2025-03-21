namespace Tutor.Infratructures.Models
{
    public class MarkAttendanceDTO
    {
        public int BookingId { get; set; }
        public bool IsAttended { get; set; }
        public string Note { get; set; }
    }

    public class LessonCompletionDTO
    {
        public string Summary { get; set; }
        public string HomeworkAssigned { get; set; }
        public string TeacherNotes { get; set; }
    }

    public class ActiveLessonDTO
    {
        public int BookingId { get; set; }
        public string StudentName { get; set; }
        public string LessonTitle { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool StudentJoined { get; set; }
        public DateTime? StudentJoinTime { get; set; }
        public LessonStatusEnum Status { get; set; }
    }

    public class LessonAttendanceHistoryDTO
    {
        public int BookingId { get; set; }
        public string LessonTitle { get; set; }
        public string InstructorName { get; set; }
        public string StudentName { get; set; }
        public DateTime ScheduledStartTime { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool? StudentAttended { get; set; }
        public string Notes { get; set; }
        public string LessonSummary { get; set; }
        public string HomeworkAssigned { get; set; }
    }

    public enum LessonStatusEnum
    {
        Scheduled,
        Started,
        InProgress,
        Completed,
        Cancelled,
        Absent
    }
}

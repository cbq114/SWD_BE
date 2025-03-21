using Tutor.Infratructures.Models;

namespace Tutor.Applications.Interfaces
{
    public interface IAttendanceService
    {
        Task<bool> AutoCompleteLesson(int bookingId);
        Task<List<ActiveLessonDTO>> GetLessonsPassedEndTime();
        Task<bool> MarkStudentAbsent(int bookingId, string reason);
        Task<List<ActiveLessonDTO>> GetLessonsWithoutStudentJoin(int minutesSinceStart);
        Task<List<ActiveLessonDTO>> GetLessonsStartingSoon(int minutesBeforeStart);
        Task<List<ActiveLessonDTO>> GetActiveLessons(string instructorUsername);
        Task<LessonAttendanceHistoryDTO> GetLessonAttendanceHistory(int bookingId);
        Task<string> StudentJoinLesson(int bookingId);
        Task<bool> EndLesson(int bookingId, LessonCompletionDTO completionDTO);
        Task<bool> MarkAttendance(MarkAttendanceDTO attendanceDTO);
        Task<bool> StartLesson(int bookingId);
        Task<bool> SendReminderToLateStudents(int minutesSinceStart);
        Task<bool> AutoMarkAbsentAfterDelay(int minutesSinceStart);
    }
}

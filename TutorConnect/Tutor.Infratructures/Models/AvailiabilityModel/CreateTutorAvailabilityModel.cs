using Tutor.Domains.Enums;

namespace Tutor.Infratructures.Models.AvailiabilityModel
{
    public class CreateTutorAvailabilityModel
    {
        public int DayOfWeek { get; set; }
        public string meetingLink { get; set; }
        public DateTime? StartTime { get; set; }

        public TutorAvailabilitityStatus Status { get; set; }
    }

    public class ScheduleModel : CreateTutorAvailabilityModel
    {
        public int TutorAvailabilityId { get; set; }
        public DateTime? EndTime { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tutor.Domains.Enums;

namespace Tutor.Domains.Entities
{
    public class LessonAttendanceDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int LessonAttendanceId { get; set; }

        public string ParticipantUsername { get; set; }

        public ParticipantRole Role { get; set; }

        public DateTime JoinTimestamp { get; set; }

        public DateTime? LeaveTimestamp { get; set; }

        public string Notes { get; set; }

        [ForeignKey("LessonAttendanceId")]
        public LessonAttendances LessonAttendance { get; set; }

        [ForeignKey("ParticipantUsername")]
        public Users User { get; set; }
    }
}
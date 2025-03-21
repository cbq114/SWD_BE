using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tutor.Domains.Enums;

namespace Tutor.Domains.Entities
{
    public class Users
    {
        [Key]
        public string UserName { get; set; }
        public int RoleId { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string Avatar { get; set; }
        [Required]
        public DateTime DOB { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public UserStatus Status { get; set; }
        public string? VerificationCode { get; set; }
        public DateTime? VerificationCodeExpiry { get; set; }

        [ForeignKey("RoleId")]
        public Roles Role { get; set; }
        public virtual Profile Profile { get; set; }
        public virtual Wallet Wallet { get; set; }
        public virtual FavoriteInstructors FavoriteInstructors { get; set; }
        public virtual ICollection<RefreshTokens> RefreshTokens { get; set; }
        public virtual ICollection<Promotions> Promotions { get; set; }
        public virtual ICollection<Lessons> Lessons { get; set; }
        public virtual ICollection<Languagues> Languagues { get; set; }
        public virtual ICollection<TutorAvailabilities> TutorAvailabilities { get; set; }
        public virtual ICollection<Bookings> Bookings { get; set; }
        public virtual ICollection<MessageRooms> MessageRooms { get; set; }
        public virtual ICollection<MessageRoomMember> MessageRoomMembers { get; set; }
        public virtual ICollection<MessageContents> MessageContents { get; set; }
        public virtual ICollection<Feedbacks> Feedbacks { get; set; }
        public virtual ICollection<Payouts> Payouts { get; set; }
        public virtual ICollection<UpgradeRequest> UpgradeRequests { get; set; }

        public virtual ICollection<LessonAttendanceDetails> AttendanceDetails { get; set; }

    }

}

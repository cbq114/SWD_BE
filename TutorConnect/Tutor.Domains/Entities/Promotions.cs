using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tutor.Domains.Enums;

namespace Tutor.Domains.Entities
{
    public class Promotions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int promotionId { get; set; }
        public string Instructor { get; set; }
        public int LessonId { get; set; }
        public PromotionStatus Status { get; set; }
        public decimal? Discount { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public int limit { get; set; }

        [ForeignKey("Instructor")]
        public Users User { get; set; }
        public virtual ICollection<PromotionUsage>? PromotionUsages { get; set; }

        [ForeignKey("LessonId")]
        public Lessons Lesson { get; set; }

    }
}

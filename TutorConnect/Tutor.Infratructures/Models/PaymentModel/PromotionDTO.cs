using System.ComponentModel.DataAnnotations;
using Tutor.Domains.Enums;
using Tutor.Shared.Helper;

namespace Tutor.Infratructures.Models.PaymentModel
{
    public class CreatePromotion
    {
        public string Instructor { get; set; }

        [Required(ErrorMessage = "LessonId is required.")]
        public int LessonId { get; set; }

        public PromotionStatus Status { get; set; }

        [Required(ErrorMessage = "Discount code is required.")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Discount is required.")]
        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
        public decimal? Discount { get; set; }

        [StringLength(500, ErrorMessage = "Description must be at most 500 characters long.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(CreatePromotion), nameof(ValidateStartDate))]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(CreatePromotion), nameof(ValidateEndDate))]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Limit is required.")]
        [Range(1, 50, ErrorMessage = "Limit must be between 1 and 50.")]
        public int Limit { get; set; }

        // Validation for StartDate
        public static ValidationResult? ValidateStartDate(DateTime? startDate, ValidationContext context)
        {
            if (startDate.HasValue && startDate.Value.Date < DateTimeHelper.GetVietnamNow().Date)
            {
                return new ValidationResult("Start date cannot be in the past.");
            }
            return ValidationResult.Success;
        }

        // Validation for EndDate
        public static ValidationResult? ValidateEndDate(DateTime? endDate, ValidationContext context)
        {
            var instance = (CreatePromotion)context.ObjectInstance;
            if (endDate.HasValue && instance.StartDate.HasValue && endDate.Value.Date < instance.StartDate.Value.Date)
            {
                return new ValidationResult("End date cannot be earlier than start date.");
            }
            return ValidationResult.Success;
        }
    }
    public class PromotionDTO : CreatePromotion
    {
        public int promotionId { get; set; }
    }
}

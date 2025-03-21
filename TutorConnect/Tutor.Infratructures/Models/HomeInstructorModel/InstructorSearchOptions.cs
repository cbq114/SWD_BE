namespace Tutor.Infratructures.Models.HomeInstructorModel
{
    public class InstructorSearchOptions
    {
        // Filter theo thông tin cá nhân
        public string? FullName { get; set; }
        public string? Country { get; set; }
        public int? LanguageId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }


        public int? MinTotalLessons { get; set; }
        public int? MinTotalStudents { get; set; }
        public decimal? MinAverageRating { get; set; }
        public bool IsDescending { get; set; } = false;

        // Phân trang
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

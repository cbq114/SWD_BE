using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.HomeInstructorModel;
using Tutor.Infratructures.Models.LessonModel;
using Tutor.Infratructures.Persistence;

namespace Tutor.Infratructures.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly TutorDBContext _dbContext;
        private readonly IMapper _mapper;
        private const string TUTOR_ROLE = "Tutor";

        public HomeRepository(TutorDBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<InstructorDetailsModelHome>> GetInstructorsForHomepageAsync(InstructorSearchOptions searchOptions)
        {
            if (searchOptions == null) throw new ArgumentNullException(nameof(searchOptions));
            if (searchOptions.PageNumber < 1) throw new ArgumentException("Page number must be greater than 0");
            if (searchOptions.PageSize < 1) throw new ArgumentException("Page size must be greater than 0");

            // Build and execute main query
            var query = BuildBaseQuery()
                .ApplySearchFilters(searchOptions)
                .ApplySorting(searchOptions.IsDescending)
                .ApplyPagination(searchOptions.PageNumber, searchOptions.PageSize);

            var instructors = await query.ToListAsync();

            // Transform results
            var instructorDetailsList = new List<InstructorDetailsModelHome>();
            foreach (var instructor in instructors)
            {
                var details = await CreateInstructorDetailsAsync(instructor);
                if (MeetsPostQueryCriteria(details, searchOptions))
                {
                    instructorDetailsList.Add(details);
                }
            }

            return instructorDetailsList;
        }

        public async Task<InstructorDetailsModelHome> GetInstructorDetailsByIdAsync(string instructorId)
        {
            if (string.IsNullOrWhiteSpace(instructorId))
                throw new ArgumentException("Instructor ID cannot be empty", nameof(instructorId));

            var instructor = await BuildBaseQuery()
                .FirstOrDefaultAsync(u => u.UserName == instructorId);

            if (instructor == null)
            {
                throw new Exception($"Instructor with ID {instructorId} not found or not active");
            }

            return await CreateInstructorDetailsAsync(instructor);
        }

        private async Task<InstructorRatingSummary> GetInstructorRatingSummaryAsync(string instructorId)
        {
            var feedbacks = await _dbContext.Feedbacks
                .AsNoTracking()
                .Where(f => f.Booking.Lesson.Instructor == instructorId &&
                           f.Status == FeedbackStatus.Approved)
                .Select(f => f.Star ?? 0)
                .ToListAsync();

            return new InstructorRatingSummary
            {
                AverageRating = feedbacks.Any() ? feedbacks.Average() : 0,
                TotalFeedbacks = feedbacks.Count
            };
        }

        private async Task<List<LessonViewModel>> GetInstructorLessonsAsync(string instructorId)
        {
            return await _dbContext.Lessons
                .AsNoTracking()
                .Include(l => l.User)
                    .ThenInclude(u => u.Languagues)
                .Where(l => l.Instructor == instructorId &&
                           l.Status == LessonStatus.Active)
                .Select(l => new LessonViewModel
                {
                    LessonId = l.LessonId,
                    Title = l.Title,
                    Level = l.Level,
                    Description = l.Description,
                    ImageUrl = l.ImageUrl,
                    CreateAt = l.CreateAt,
                    Status = l.Status,
                    LearningObjectives = l.LearningObjectives,
                    Material = l.Material,
                    LanguageName = l.User.Languagues.FirstOrDefault().LanguageName
                })
                .ToListAsync();
        }

        private IQueryable<Users> BuildBaseQuery()
        {
            return _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Profile)
                    .ThenInclude(p => p.Certifications)
                .Include(u => u.Languagues)
                .Include(u => u.Role)
                .Include(u => u.Lessons)
                    .ThenInclude(l => l.Bookings)
                .Where(u => u.Role.RoleName == TUTOR_ROLE &&
                           u.Status == UserStatus.verified &&
                           u.Profile.TutorStatus == TutorStatus.Avaliable);
        }

        private async Task<InstructorDetailsModelHome> CreateInstructorDetailsAsync(Users instructor)
        {
            var ratingSummary = await GetInstructorRatingSummaryAsync(instructor.UserName);
            var lessons = await GetInstructorLessonsAsync(instructor.UserName);

            var totalStudents = instructor.Lessons
                .SelectMany(l => l.Bookings ?? Enumerable.Empty<Bookings>())
                .Select(b => b.customer)
                .Distinct()
                .Count();

            return new InstructorDetailsModelHome
            {
                UserName = instructor.UserName,
                FullName = instructor.FullName,
                Email = instructor.Email,
                PhoneNumber = instructor.PhoneNumber,
                Avatar = instructor.Avatar,
                Address = instructor.Profile.Address,
                TeachingExperience = instructor.Profile.TeachingExperience,
                Education = instructor.Profile.Education,
                Country = instructor.Profile.Country,
                LanguageId = instructor.Profile.LanguageId,
                Price = instructor.Profile.Price ?? 0,
                TotalLessons = lessons.Count,
                TotalStudents = totalStudents,
                AverageRating = ratingSummary.AverageRating,
                TotalFeedbacks = ratingSummary.TotalFeedbacks,
                Lessons = lessons
            };
        }

        private static bool MeetsPostQueryCriteria(InstructorDetailsModelHome instructor, InstructorSearchOptions searchOptions)
        {
            return (!searchOptions.MinTotalLessons.HasValue || instructor.TotalLessons >= searchOptions.MinTotalLessons.Value) &&
                   (!searchOptions.MinTotalStudents.HasValue || instructor.TotalStudents >= searchOptions.MinTotalStudents.Value) &&
                   (!searchOptions.MinAverageRating.HasValue || (decimal)instructor.AverageRating >= searchOptions.MinAverageRating.Value);
        }
    }

    // Extension methods for query building
    public static class QueryableExtensions
    {
        public static IQueryable<Users> ApplySearchFilters(this IQueryable<Users> query, InstructorSearchOptions searchOptions)
        {
            if (!string.IsNullOrWhiteSpace(searchOptions.FullName))
            {
                query = query.Where(u => EF.Functions.Like(u.FullName, $"%{searchOptions.FullName}%"));
            }

            if (!string.IsNullOrWhiteSpace(searchOptions.Country))
            {
                query = query.Where(u => u.Profile.Country == searchOptions.Country);
            }

            if (searchOptions.LanguageId.HasValue)
            {
                query = query.Where(u => u.Profile.LanguageId == searchOptions.LanguageId);
            }

            if (searchOptions.MinPrice.HasValue)
            {
                query = query.Where(u => u.Profile.Price.HasValue &&
                                       u.Profile.Price.Value >= searchOptions.MinPrice.Value);
            }

            if (searchOptions.MaxPrice.HasValue)
            {
                query = query.Where(u => u.Profile.Price.HasValue &&
                                       u.Profile.Price.Value <= searchOptions.MaxPrice.Value);
            }

            return query;
        }

        public static IQueryable<Users> ApplySorting(this IQueryable<Users> query, bool isDescending)
        {
            return isDescending
                ? query.OrderByDescending(u => u.Profile.Price)
                : query.OrderBy(u => u.Profile.Price);
        }

        public static IQueryable<Users> ApplyPagination(this IQueryable<Users> query, int pageNumber, int pageSize)
        {
            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
    }
}
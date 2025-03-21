using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Configurations;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.LessonModel;
using Tutor.Infratructures.Persistence;

namespace Tutor.Infratructures.Repositories
{
    public class LessonRepository : Repository<Lessons>, ILessonRepository
    {
        private readonly TutorDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LessonRepository> _logger;
        private readonly IConfiguration _configuration;

        public LessonRepository(
            TutorDBContext dbContext,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<LessonRepository> logger,
            IConfiguration configuration)
            : base(dbContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<string> AddLesson(CreateLessonModel model, string username, IFormFile imgUrl)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var activeTutor = await _dbContext.Profile.FirstOrDefaultAsync(x => x.UserName == username);
                if (activeTutor == null || activeTutor.TutorStatus == Domains.Enums.TutorStatus.NotAvaliable)
                {
                    throw new InvalidOperationException("Tutor not found.");
                }

                var existingLesson = await Entities.AnyAsync(x => x.Title == model.Title);
                if (existingLesson)
                {
                    throw new InvalidOperationException("Lesson with the same name already exists.");
                }

                string img = null;
                if (imgUrl != null)
                {
                    var cloudinaryService = new CloundinaryRepository(_configuration);
                    img = await cloudinaryService.UploadImage(imgUrl);
                }

                var lesson = _mapper.Map<Lessons>(model);
                lesson.Instructor = username;
                lesson.ImageUrl = img;

                // Gán bài học cha (nếu có)
                if (model.ParentLessonId.HasValue)
                {
                    var parentLesson = await Entities.FindAsync(model.ParentLessonId);
                    if (parentLesson == null)
                    {
                        throw new InvalidOperationException("Parent lesson not found.");
                    }
                    lesson.ParentLessonId = model.ParentLessonId;
                }

                await Entities.AddAsync(lesson);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Lesson added successfully: {LessonTitle}", lesson.Title);
                return "Lesson added successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding lesson: {LessonTitle}", model.Title);
                throw;
            }
        }

        public async Task<string> DeleteLesson(int lessonId)
        {
            var lesson = await Entities.FindAsync(lessonId);
            if (lesson == null)
            {
                throw new InvalidOperationException("Lesson not found.");
            }

            Entities.Remove(lesson);
            await _unitOfWork.SaveChangesAsync();
            return "Lesson deleted successfully.";
        }

        public async Task<LessonViewModel> GetLessonById(int lessonId)
        {
            var lesson = await Entities.Include(l => l.ParentLesson)
                                       .FirstOrDefaultAsync(x => x.LessonId == lessonId);
            if (lesson == null)
            {
                throw new InvalidOperationException("Lesson not found.");
            }

            var lessonViewModel = _mapper.Map<LessonViewModel>(lesson);
            if (lesson.ParentLesson != null)
            {
                lessonViewModel.ParentLessonId = lesson.ParentLesson.LessonId;
                lessonViewModel.ParentLessonTitle = lesson.ParentLesson.Title;
            }

            return lessonViewModel;
        }


        public async Task<List<LessonViewModel>> GetLessonByTutor(string username)
        {
            var lessons = await Entities.Where(x => x.Instructor == username).ToListAsync();
            return _mapper.Map<List<LessonViewModel>>(lessons);
        }

        public async Task<Lessons> GetLessonsById(int lessonId)
        {
            var lesson = await Entities.FirstOrDefaultAsync(x => x.LessonId == lessonId);
            if (lesson == null)
            {
                throw new InvalidOperationException("Lesson not found.");
            }
            return lesson;
        }

        public async Task<string> UpdateLesson(CreateLessonModel model, string username, IFormFile imageUrl, int lessonId)
        {
            var lesson = await Entities.FirstOrDefaultAsync(x => x.LessonId == lessonId);
            if (lesson == null)
            {
                throw new InvalidOperationException("Lesson not found.");
            }
            var updatedLesson = _mapper.Map(model, lesson);
            if (imageUrl != null)
            {
                var cloudinaryService = new CloundinaryRepository(_configuration);
                updatedLesson.ImageUrl = await cloudinaryService.UploadImage(imageUrl);
            }
            await _unitOfWork.SaveChangesAsync();
            return "Lesson updated successfully.";
        }

    }
}

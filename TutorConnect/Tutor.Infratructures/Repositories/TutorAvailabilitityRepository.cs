using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.AvailiabilityModel;
using Tutor.Infratructures.Persistence;

namespace Tutor.Infratructures.Repositories
{
    public class TutorAvailabilitityRepository : ITutorAvailabilitityRepository
    {
        private readonly IMapper _mapper;
        private readonly TutorDBContext _dbContext;
        //private readonly ILanguageRepository _languageRepository;
        public TutorAvailabilitityRepository(IMapper mapper, TutorDBContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            //_languageRepository = languageRepository;
        }

        public async Task<string> AddTutorAvailability(CreateTutorAvailabilityModel model, string instructor)
        {
            try
            {
                if (!model.StartTime.HasValue)
                    throw new Exception("StartTime is required");

                var startTime = model.StartTime.Value;
                var endTime = startTime.AddHours(1); // Slot mặc định là 1 tiếng

                // Giới hạn giờ cho phép đặt lịch: 6h sáng đến 10h tối
                if (startTime.TimeOfDay < TimeSpan.FromHours(6) || startTime.TimeOfDay >= TimeSpan.FromHours(22))
                    throw new Exception("Schedules must be between 6:00 AM and 10:00 PM");

                // Kiểm tra xem đã có lịch trùng giờ chưa
                var existingSchedule = await _dbContext.TutorAvailabilities
                    .FirstOrDefaultAsync(x => x.Instructor == instructor && x.StartTime == startTime);
                if (existingSchedule != null)
                    throw new Exception("Schedule already exists at this time");

                // Lấy lịch gần nhất trước thời gian cần thêm
                var lastSchedule = await _dbContext.TutorAvailabilities
                    .Where(x => x.Instructor == instructor && x.StartTime < startTime)
                    .OrderByDescending(x => x.StartTime)
                    .FirstOrDefaultAsync();

                if (lastSchedule != null && lastSchedule.EndTime.HasValue)
                {
                    var minNextStartTime = lastSchedule.EndTime.Value.AddMinutes(30);
                    if (startTime < minNextStartTime)
                        throw new Exception($"New schedule must be at least 30 minutes after the last schedule ends at {lastSchedule.EndTime.Value:HH:mm}");
                }

                // Tạo lịch mới
                var tutorAvailability = _mapper.Map<TutorAvailabilities>(model);
                tutorAvailability.Instructor = instructor;
                tutorAvailability.EndTime = endTime;

                await _dbContext.TutorAvailabilities.AddAsync(tutorAvailability);
                await _dbContext.SaveChangesAsync();

                return "Add schedule successful!";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<bool> DeleteTutorAvailability(string tutor, int id)
        {
            var availability = await _dbContext.TutorAvailabilities
                .Include(a => a.Bookings)
                .FirstOrDefaultAsync(a => a.TutorAvailabilityId == id && a.Instructor == tutor);

            if (availability == null)
            {
                return false;
            }

            // Check if there are any existing bookings
            if (availability.Bookings.Any())
            {
                throw new InvalidOperationException("Cannot delete availability with existing bookings.");
            }

            // Delete the availability
            _dbContext.TutorAvailabilities.Remove(availability);

            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("An error occurred while deleting the availability.", ex);
            }
        }


        public async Task<List<ScheduleModel>> GetAllScheduleforStudentView(string instructor)
        {
            var schedule = await _dbContext.TutorAvailabilities.Include(u => u.User)
                                          .ThenInclude(s => s.Languagues)
                                          .Where(x => x.Instructor == instructor && x.Status != TutorAvailabilitityStatus.Unavailable)
                                          .Select(x => new ScheduleModel
                                          {
                                              TutorAvailabilityId = x.TutorAvailabilityId,
                                              meetingLink = x.meetingLink,
                                              StartTime = x.StartTime,
                                              EndTime = x.EndTime,
                                              Status = x.Status,
                                          })
                                          .ToListAsync();

            if (schedule == null || !schedule.Any())
                throw new Exception("Schedule not found");

            return schedule;
        }
        public async Task<List<ScheduleModel>> GetAllScheduleforTutorView(string instructor)
        {
            var schedule = await _dbContext.TutorAvailabilities.Include(u => u.User)
                                          .ThenInclude(s => s.Languagues)
                                          .Where(x => x.Instructor == instructor)
                                          .Select(x => new ScheduleModel
                                          {
                                              TutorAvailabilityId = x.TutorAvailabilityId,
                                              meetingLink = x.meetingLink,
                                              StartTime = x.StartTime,
                                              EndTime = x.EndTime,
                                              Status = x.Status,
                                          })
                                          .ToListAsync();

            if (schedule == null || !schedule.Any())
                throw new Exception("Schedule not found");

            return schedule;
        }

        public async Task<TutorAvailabilities> GetScheduleById(int id)
        {
            var schedule = await _dbContext.TutorAvailabilities.FirstOrDefaultAsync(x => x.TutorAvailabilityId == id);
            if (schedule == null)
                throw new Exception("Schedule not found");

            return schedule;
        }

        public async Task<TutorAvailabilities> UpdateTutorAvailability(CreateTutorAvailabilityModel model, int id)
        {
            var existingSchedule = await _dbContext.TutorAvailabilities.FirstOrDefaultAsync(x => x.TutorAvailabilityId == id);
            if (existingSchedule == null)
                throw new Exception("Schedule not found");
            if (existingSchedule.Status == TutorAvailabilitityStatus.booked)
                throw new Exception("Cannot update an Booked schedule");
            existingSchedule.StartTime = model.StartTime;
            existingSchedule.Status = model.Status;

            // Automatically set EndTime based on StartTime if it's not already set
            if (existingSchedule.StartTime.HasValue)
            {
                existingSchedule.EndTime = existingSchedule.StartTime.Value.AddHours(1);
            }

            _dbContext.TutorAvailabilities.Update(existingSchedule);
            await _dbContext.SaveChangesAsync();
            return existingSchedule;
        }

        public async Task ChangeTutorAvailStatus(int id, TutorAvailabilitityStatus status)
        {
            var tutorAvail = await GetScheduleById(id);
            if (tutorAvail == null)
                throw new Exception("Schedule not found");
            //if (tutorAvail.Status == TutorAvailabilitityStatus.booked)
            //    throw new Exception("Cannot update an Booked schedule");
            tutorAvail.Status = status;
            await _dbContext.SaveChangesAsync();
        }
    }

}
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tutor.Applications.Interfaces;

namespace Tutor.Applications.BackgroundServices
{
    public class AttendanceAutomationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AttendanceAutomationService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public AttendanceAutomationService(
    IServiceProvider serviceProvider,
    ILogger<AttendanceAutomationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Attendance Automation Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessAttendanceRules();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing attendance rules");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Attendance Automation Service is stopping.");
        }
        private async Task ProcessAttendanceRules()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var attendanceService = scope.ServiceProvider.GetRequiredService<IAttendanceService>();

                // Send reminders to students who haven't joined 5 minutes after start time
                await attendanceService.SendReminderToLateStudents(5);

                // Mark students as absent if they haven't joined 10 minutes after start time
                await attendanceService.AutoMarkAbsentAfterDelay(10);

                // Auto-complete lessons that have passed their end time
                await attendanceService.GetLessonsPassedEndTime().ContinueWith(async (lessonsTask) =>
                {
                    var lessons = await lessonsTask;
                    foreach (var lesson in lessons)
                    {
                        await attendanceService.AutoCompleteLesson(lesson.BookingId);
                    }
                });
            }
        }
    }
}
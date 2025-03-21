using Microsoft.Extensions.Logging;
using Tutor.Applications.Interfaces;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Configurations;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models;
using Tutor.Shared.Helper;

namespace Tutor.Applications.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ILessonAttendanceRepository _attendanceRepository;
        private readonly ILessonAttendanceDetailsRepository _attendanceDetailsRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly ITutorAvailabilitityRepository _availabilityRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AttendanceService> _logger;
        private readonly IEmailSender _emailSender;

        public AttendanceService(ILessonAttendanceRepository attendanceRepository, ILessonAttendanceDetailsRepository attendanceDetailsRepository, IBookingRepository bookingRepository, ILessonRepository lessonRepository, ITutorAvailabilitityRepository availabilityRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<AttendanceService> logger, IEmailSender emailSender)
        {
            _attendanceRepository = attendanceRepository;
            _attendanceDetailsRepository = attendanceDetailsRepository;
            _bookingRepository = bookingRepository;
            _lessonRepository = lessonRepository;
            _availabilityRepository = availabilityRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _emailSender = emailSender;
        }
        public async Task<bool> StartLesson(int bookingId)
        {
            //try
            //{
            var booking = await _bookingRepository.GetBookingById(bookingId);
            if (booking == null || booking.Status != BookingStatus.Accepted)
                return false;

            var availability = await _availabilityRepository.GetScheduleById(booking.AvailabilityId);
            if (availability == null)
                return false;

            // Check if the lesson is within start time window (15 min before to 15 min after)
            var now = DateTime.UtcNow.AddHours(7); ;
            var startTimeWindow = availability.StartTime.Value.AddMinutes(-15);
            var endTimeWindow = availability.StartTime.Value.AddMinutes(15);

            if (now < startTimeWindow || now > endTimeWindow)
                return false;

            // Create attendance record
            var attendance = new LessonAttendances
            {
                BookingId = bookingId,
                IsAttended = false, // Will be updated when student joins
                note = "Start",
                CreateAt = now
            };

            await _attendanceRepository.Add(attendance);

            // Create tutor's attendance detail
            var attendanceId = attendance.LessonAttendanceId;
            var tutorDetail = new LessonAttendanceDetails
            {
                LessonAttendanceId = attendanceId,
                ParticipantUsername = booking.Lesson.Instructor,
                Role = ParticipantRole.Tutor,
                JoinTimestamp = now,
                Notes = "Lesson started"
            };

            await _attendanceDetailsRepository.Add(tutorDetail);

            // Update booking status
            booking.Status = BookingStatus.InProgress;
            await _bookingRepository.Update(booking);

            return true;

        }

        public async Task<bool> MarkAttendance(MarkAttendanceDTO attendanceDTO)
        {

            var attendance = await _attendanceRepository.GetByBookingIdAsync(attendanceDTO.BookingId);
            if (attendance == null)
                return false;

            attendance.IsAttended = attendanceDTO.IsAttended;
            attendance.note = attendanceDTO.Note;

            await _attendanceRepository.Update(attendance);

            return true;
        }

        public async Task<bool> EndLesson(int bookingId, LessonCompletionDTO completionDTO)
        {
            var booking = await _bookingRepository.GetBookingById(bookingId);
            if (booking == null || booking.Status != BookingStatus.InProgress)
                return false;

            var attendance = await _attendanceRepository.GetByBookingIdAsync(bookingId);
            if (attendance == null)
                return false;

            // Update attendance with completion notes
            attendance.note = $"Summary: {completionDTO.Summary}\nHomework: {completionDTO.HomeworkAssigned}\nTeacher Notes: {completionDTO.TeacherNotes}";
            await _attendanceRepository.Update(attendance);

            // Update booking status
            booking.Status = BookingStatus.Completed;
            await _bookingRepository.Update(booking);

            // Update tutor attendance details
            var details = await _attendanceDetailsRepository.GetByAttendanceIdAsync(attendance.LessonAttendanceId);
            var tutorDetail = details.FirstOrDefault(d => d.Role == ParticipantRole.Tutor);
            if (tutorDetail != null)
            {
                tutorDetail.LeaveTimestamp = DateTimeHelper.GetVietnamNow();
                tutorDetail.Notes += "\nLesson ended";
                await _attendanceDetailsRepository.Update(tutorDetail);
            }
            return true;
        }

        public async Task<string> StudentJoinLesson(int bookingId)
        {
            var booking = await _bookingRepository.GetBookingById(bookingId);
            if (booking == null || booking.Status != BookingStatus.InProgress)
                return "";

            var attendance = await _attendanceRepository.GetByBookingIdAsync(bookingId);
            if (attendance == null)
                return "";

            var avalible = await _availabilityRepository.GetScheduleById(booking.AvailabilityId);
            var meetLink = "";
            if (avalible != null)
            {
                meetLink = avalible.meetingLink;
            }

            // Update attendance status
            attendance.IsAttended = true;
            await _attendanceRepository.Update(attendance);

            // Add student attendance detail
            var studentDetail = new LessonAttendanceDetails
            {
                LessonAttendanceId = attendance.LessonAttendanceId,
                ParticipantUsername = booking.customer,
                Role = ParticipantRole.Student,
                JoinTimestamp = DateTimeHelper.GetVietnamNow(),
                Notes = meetLink
            };

            await _attendanceDetailsRepository.Update(studentDetail);

            return meetLink;
        }
        public async Task<LessonAttendanceHistoryDTO> GetLessonAttendanceHistory(int bookingId)
        {
            var booking = await _bookingRepository.GetBookingById(bookingId);
            if (booking == null)
                return null;

            var lesson = await _lessonRepository.GetLessonById(booking.LessonId);
            var availability = await _availabilityRepository.GetScheduleById(booking.AvailabilityId);
            var attendance = await _attendanceRepository.GetByBookingIdAsync(bookingId);

            if (lesson == null || availability == null)
                return null;

            var studentUser = await _userRepository.GetByUsernameAsync(booking.customer);
            var tutorUser = await _userRepository.GetByUsernameAsync(lesson.Instructor);

            var history = new LessonAttendanceHistoryDTO
            {
                BookingId = bookingId,
                LessonTitle = lesson.Title,
                InstructorName = tutorUser?.FullName ?? lesson.Instructor,
                StudentName = studentUser?.FullName ?? booking.customer,
                ScheduledStartTime = availability.StartTime.Value
            };

            if (attendance != null)
            {
                var details = await _attendanceDetailsRepository.GetByAttendanceIdAsync(attendance.LessonAttendanceId);

                var tutorDetail = details.FirstOrDefault(d => d.Role == ParticipantRole.Tutor);
                var studentDetail = details.FirstOrDefault(d => d.Role == ParticipantRole.Student);

                history.ActualStartTime = tutorDetail?.JoinTimestamp;
                history.EndTime = tutorDetail?.LeaveTimestamp;
                history.StudentAttended = attendance.IsAttended;
                history.Notes = attendance.note;

                // Parse notes if they exist
                if (!string.IsNullOrEmpty(attendance.note) && attendance.note.Contains("Summary:"))
                {
                    var parts = attendance.note.Split('\n');
                    history.LessonSummary = parts[0].Replace("Summary: ", "");

                    if (parts.Length > 1)
                        history.HomeworkAssigned = parts[1].Replace("Homework: ", "");
                }
            }

            return history;
        }

        public async Task<List<ActiveLessonDTO>> GetActiveLessons(string instructorUsername)
        {

            // Get all lessons by this instructor that are currently active
            var now = DateTimeHelper.GetVietnamNow();

            var activeBookings = await _bookingRepository.GetActiveBookingsForTutorAsync(instructorUsername, now);

            if (!activeBookings.Any())
                return new List<ActiveLessonDTO>();

            var result = new List<ActiveLessonDTO>();

            foreach (var booking in activeBookings)
            {
                var lesson = await _lessonRepository.GetLessonById(booking.LessonId);
                var availability = await _availabilityRepository.GetScheduleById(booking.AvailabilityId);
                var attendance = await _attendanceRepository.GetByBookingIdAsync(booking.BookingId);
                var student = await _userRepository.GetByUsernameAsync(booking.customer);

                var activeLesson = new ActiveLessonDTO
                {
                    BookingId = booking.BookingId,
                    LessonTitle = lesson?.Title ?? "Unknown Lesson",
                    StudentName = student?.FullName ?? booking.customer,
                    StartTime = availability?.StartTime ?? DateTime.MinValue,
                    EndTime = availability?.EndTime ?? DateTime.MinValue,
                    StudentJoined = attendance?.IsAttended ?? false,
                    Status = MapBookingStatusToLessonStatus(booking.Status)
                };

                if (attendance != null)
                {
                    var details = await _attendanceDetailsRepository.GetByAttendanceIdAsync(attendance.LessonAttendanceId);
                    var studentDetail = details.FirstOrDefault(d => d.Role == ParticipantRole.Student);

                    if (studentDetail != null)
                    {
                        activeLesson.StudentJoined = true;
                        activeLesson.StudentJoinTime = studentDetail.JoinTimestamp;
                    }
                }

                result.Add(activeLesson);
            }

            return result;

        }

        // Methods for the background service
        public async Task<List<ActiveLessonDTO>> GetLessonsStartingSoon(int minutesBeforeStart)
        {
            try
            {
                var threshold = DateTimeHelper.GetVietnamNow().AddMinutes(minutesBeforeStart);
                var bookingIds = await _attendanceRepository.GetBookingIdsStartingSoonAsync(threshold);

                var result = new List<ActiveLessonDTO>();

                foreach (var bookingId in bookingIds)
                {
                    var booking = await _bookingRepository.GetBookingById(bookingId);
                    if (booking == null) continue;

                    var lesson = await _lessonRepository.GetLessonById(booking.LessonId);
                    var availability = await _availabilityRepository.GetScheduleById(booking.AvailabilityId);
                    var student = await _userRepository.GetByUsernameAsync(booking.customer);

                    result.Add(new ActiveLessonDTO
                    {
                        BookingId = booking.BookingId,
                        LessonTitle = lesson?.Title ?? "Unknown Lesson",
                        StudentName = student?.FullName ?? booking.customer,
                        StartTime = availability?.StartTime ?? DateTime.MinValue,
                        EndTime = availability?.EndTime ?? DateTime.MinValue,
                        Status = LessonStatusEnum.Scheduled
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lessons starting soon");
                return new List<ActiveLessonDTO>();
            }
        }

        public async Task<List<ActiveLessonDTO>> GetLessonsWithoutStudentJoin(int minutesSinceStart)
        {
            try
            {
                var threshold = DateTimeHelper.GetVietnamNow().AddMinutes(-minutesSinceStart);
                var bookingIds = await _attendanceRepository.GetBookingIdsWithoutStudentJoinAsync(threshold);

                var result = new List<ActiveLessonDTO>();

                foreach (var bookingId in bookingIds)
                {
                    var booking = await _bookingRepository.GetBookingById(bookingId);
                    if (booking == null || booking.Status != BookingStatus.InProgress) continue;

                    var lesson = await _lessonRepository.GetLessonById(booking.LessonId);
                    var availability = await _availabilityRepository.GetScheduleById(booking.AvailabilityId);
                    var student = await _userRepository.GetByUsernameAsync(booking.customer);

                    result.Add(new ActiveLessonDTO
                    {
                        BookingId = booking.BookingId,
                        LessonTitle = lesson?.Title ?? "Unknown Lesson",
                        StudentName = student?.FullName ?? booking.customer,
                        StartTime = availability?.StartTime ?? DateTime.MinValue,
                        EndTime = availability?.EndTime ?? DateTime.MinValue,
                        Status = LessonStatusEnum.Started
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lessons without student join");
                return new List<ActiveLessonDTO>();
            }
        }

        public async Task<bool> MarkStudentAbsent(int bookingId, string reason)
        {
            try
            {
                var booking = await _bookingRepository.GetBookingById(bookingId);
                if (booking == null || booking.Status != BookingStatus.InProgress)
                    return false;

                var attendance = await _attendanceRepository.GetByBookingIdAsync(bookingId);
                if (attendance == null)
                    return false;

                // Update attendance status
                attendance.IsAttended = false;
                attendance.note = reason;
                await _attendanceRepository.Update(attendance);

                // Update booking status
                booking.Status = BookingStatus.Absent;
                await _bookingRepository.Update(booking);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking student absent for booking {BookingId}", bookingId);
                return false;
            }
        }

        public async Task<List<ActiveLessonDTO>> GetLessonsPassedEndTime()
        {
            try
            {
                var now = DateTimeHelper.GetVietnamNow();
                var bookingIds = await _attendanceRepository.GetBookingIdsPastEndTimeAsync(now);

                var result = new List<ActiveLessonDTO>();

                foreach (var bookingId in bookingIds)
                {
                    var booking = await _bookingRepository.GetBookingById(bookingId);
                    if (booking == null || booking.Status != BookingStatus.InProgress) continue;

                    var lesson = await _lessonRepository.GetLessonById(booking.LessonId);
                    var availability = await _availabilityRepository.GetScheduleById(booking.AvailabilityId);
                    var student = await _userRepository.GetByUsernameAsync(booking.customer);

                    result.Add(new ActiveLessonDTO
                    {
                        BookingId = booking.BookingId,
                        LessonTitle = lesson?.Title ?? "Unknown Lesson",
                        StudentName = student?.FullName ?? booking.customer,
                        StartTime = availability?.StartTime ?? DateTime.MinValue,
                        EndTime = availability?.EndTime ?? DateTime.MinValue,
                        Status = LessonStatusEnum.InProgress
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lessons past end time");
                return new List<ActiveLessonDTO>();
            }
        }

        public async Task<bool> AutoCompleteLesson(int bookingId)
        {
            try
            {
                var booking = await _bookingRepository.GetBookingById(bookingId);
                if (booking == null || booking.Status != BookingStatus.InProgress)
                    return false;

                var attendance = await _attendanceRepository.GetByBookingIdAsync(bookingId);
                if (attendance == null)
                    return false;

                // Update attendance with auto-completion note
                var existingNote = attendance.note ?? "";
                attendance.note = existingNote + "\nAuto-completed as the scheduled end time has passed.";
                await _attendanceRepository.Update(attendance);

                // Update booking status
                booking.Status = BookingStatus.Completed;
                await _bookingRepository.Update(booking);

                // Update attendance details
                var details = await _attendanceDetailsRepository.GetByAttendanceIdAsync(attendance.LessonAttendanceId);
                foreach (var detail in details)
                {
                    if (detail.LeaveTimestamp == null)
                    {
                        detail.LeaveTimestamp = DateTimeHelper.GetVietnamNow();
                        detail.Notes += "\nAuto-ended lesson";
                        await _attendanceDetailsRepository.Update(detail);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error auto-completing lesson for booking {BookingId}", bookingId);
                return false;
            }
        }


        public async Task<bool> SendReminderToLateStudents(int minutesSinceStart)
        {
            try
            {
                var threshold = DateTimeHelper.GetVietnamNow().AddMinutes(-minutesSinceStart);
                var bookingIds = await _attendanceRepository.GetBookingIdsWithoutStudentJoinAsync(threshold);

                foreach (var bookingId in bookingIds)
                {
                    var booking = await _bookingRepository.GetBookingById(bookingId);
                    if (booking == null || booking.Status != BookingStatus.InProgress) continue;

                    var lesson = await _lessonRepository.GetLessonById(booking.LessonId);
                    var availability = await _availabilityRepository.GetScheduleById(booking.AvailabilityId);
                    var student = await _userRepository.GetByUsernameAsync(booking.customer);

                    if (student == null || string.IsNullOrEmpty(student.Email)) continue;

                    // Check if notification has already been sent (could store this in a new field on attendance)
                    var attendance = await _attendanceRepository.GetByBookingIdAsync(bookingId);
                    if (attendance == null || attendance.note?.Contains("Reminder sent") == true) continue;

                    // Update attendance to mark that reminder was sent
                    attendance.note = (attendance.note ?? "") + "\nReminder sent at " + DateTimeHelper.GetVietnamNow().ToString();
                    await _attendanceRepository.Update(attendance);

                    // Send email reminder
                    string subject = $"Reminder: Your lesson '{lesson?.Title}' has started";
                    string message = $@"
                        <div style='font-family: Arial, sans-serif; padding: 20px; max-width: 600px; margin: auto;'>
                            <h2 style='color: #d9534f;'>Urgent: Your Lesson Has Started</h2>
                            <p>Dear {student.FullName},</p>
                            <p>Your scheduled lesson <strong>{lesson?.Title}</strong> started at {availability?.StartTime?.ToString("HH:mm")} UTC.</p>
                            <p>Your tutor is waiting for you. Please join as soon as possible or the lesson may be marked as missed.</p>
                            <p>If you're having technical difficulties, please contact support immediately.</p>
                            <p>Best regards,<br>The Tutoring Team</p>
                        </div>";

                    await _emailSender.EmailSendAsync(student.Email, subject, message);
                    _logger.LogInformation("Sent reminder email to student {StudentName} for booking {BookingId}", student.FullName, bookingId);
                }

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending reminders to late students");
                return false;
            }
        }
        public async Task<bool> AutoMarkAbsentAfterDelay(int minutesSinceStart)
        {
            try
            {
                var threshold = DateTimeHelper.GetVietnamNow().AddMinutes(-minutesSinceStart);
                var bookingIds = await _attendanceRepository.GetBookingIdsWithoutStudentJoinAsync(threshold);

                foreach (var bookingId in bookingIds)
                {
                    var booking = await _bookingRepository.GetBookingById(bookingId);
                    if (booking == null || booking.Status != BookingStatus.InProgress) continue;

                    var attendance = await _attendanceRepository.GetByBookingIdAsync(bookingId);
                    if (attendance == null) continue;

                    // Get lesson and user info for email notification
                    var lesson = await _lessonRepository.GetLessonById(booking.LessonId);
                    var student = await _userRepository.GetByUsernameAsync(booking.customer);
                    var tutor = await _userRepository.GetByUsernameAsync(lesson?.Instructor);

                    // Mark student as absent
                    attendance.IsAttended = false;
                    attendance.note = (attendance.note ?? "") + $"\nAutomatically marked as absent after {minutesSinceStart} minutes of no-show.";
                    await _attendanceRepository.Update(attendance);

                    // Update booking status
                    booking.Status = BookingStatus.Absent;
                    await _bookingRepository.Update(booking);

                    // Update tutor attendance details
                    var details = await _attendanceDetailsRepository.GetByAttendanceIdAsync(attendance.LessonAttendanceId);
                    var tutorDetail = details.FirstOrDefault(d => d.Role == ParticipantRole.Tutor);
                    if (tutorDetail != null)
                    {
                        tutorDetail.LeaveTimestamp = DateTimeHelper.GetVietnamNow();
                        tutorDetail.Notes += $"\nLesson auto-ended due to student absence after {minutesSinceStart} minutes";
                        await _attendanceDetailsRepository.Update(tutorDetail);
                    }

                    // Send email notification to student
                    if (student != null && !string.IsNullOrEmpty(student.Email))
                    {
                        string subject = $"Missed Lesson: {lesson?.Title}";
                        string message = $@"
                            <div style='font-family: Arial, sans-serif; padding: 20px; max-width: 600px; margin: auto;'>
                                <h2 style='color: #d9534f;'>Missed Lesson</h2>
                                <p>Dear {student.FullName},</p>
                                <p>We noticed you didn't join your scheduled lesson <strong>{lesson?.Title}</strong>.</p>
                                <p>After waiting {minutesSinceStart} minutes, the system has automatically marked you as absent.</p>
                                <p>Please note that this may affect your attendance record and could incur cancellation fees according to our policy.</p>
                                <p>If you believe this is an error or had technical difficulties, please contact support.</p>
                                <p>Best regards,<br>The Tutoring Team</p>
                            </div>";

                        await _emailSender.EmailSendAsync(student.Email, subject, message);
                    }

                    // Send email notification to tutor
                    if (tutor != null && !string.IsNullOrEmpty(tutor.Email))
                    {
                        string subject = $"Student No-Show: {lesson?.Title}";
                        string message = $@"
                            <div style='font-family: Arial, sans-serif; padding: 20px; max-width: 600px; margin: auto;'>
                                <h2 style='color: #5bc0de;'>Student Absence Notification</h2>
                                <p>Dear {tutor.FullName},</p>
                                <p>Your student {student?.FullName ?? booking.customer} did not join the scheduled lesson <strong>{lesson?.Title}</strong>.</p>
                                <p>After waiting {minutesSinceStart} minutes, the system has automatically marked the student as absent and ended the lesson.</p>
                                <p>According to our policy, you will still receive compensation for this time slot.</p>
                                <p>Best regards,<br>The Tutoring Team</p>
                            </div>";

                        await _emailSender.EmailSendAsync(tutor.Email, subject, message);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error automatically marking students as absent");
                return false;
            }
        }

        private LessonStatusEnum MapBookingStatusToLessonStatus(BookingStatus status)
        {
            return status switch
            {
                BookingStatus.Accepted => LessonStatusEnum.Scheduled,
                BookingStatus.InProgress => LessonStatusEnum.InProgress,
                BookingStatus.Completed => LessonStatusEnum.Completed,
                BookingStatus.Cancelled => LessonStatusEnum.Cancelled,
                BookingStatus.Absent => LessonStatusEnum.Absent,
                _ => LessonStatusEnum.Scheduled
            };
        }
    }
}

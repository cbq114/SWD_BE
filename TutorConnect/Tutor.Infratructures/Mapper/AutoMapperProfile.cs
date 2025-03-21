using Tutor.Domains.Entities;
using Tutor.Infratructures.Models.Authen;
using Tutor.Infratructures.Models.AvailiabilityModel;
using Tutor.Infratructures.Models.DashboardModel;
using Tutor.Infratructures.Models.LessonModel;
using Tutor.Infratructures.Models.Message;
using Tutor.Infratructures.Models.PaymentModel;
using Tutor.Infratructures.Models.UpgradeModel;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.Infratructures.Mapper
{
    public class AutoMapperProfile : AutoMapper.Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Bookings, BookingViewModel>()
    .ForMember(dest => dest.CustomerUserName, opt => opt.MapFrom(src => src.customer))
    .ForMember(dest => dest.CustomerFullName, opt => opt.MapFrom(src => src.User.FullName))
    .ForMember(dest => dest.LessonTitle, opt => opt.MapFrom(src => src.Lesson.Title))
    .ForMember(dest => dest.InstructorUserName, opt => opt.MapFrom(src => src.Lesson.Instructor))
    .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.Created))
    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // Payment Mapping
            CreateMap<Payments, PaymentViewModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // Refund Mapping
            CreateMap<Refunds, RefundViewModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // Payout Mapping
            CreateMap<Payouts, PayoutViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PayoutDate, opt => opt.MapFrom(src => src.PayoutDate))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason));

            // Promotion
            CreateMap<Promotions, PromotionDTO>().ReverseMap();
            CreateMap<Promotions, CreatePromotion>().ReverseMap();

            // Feedback Mapping
            CreateMap<Feedbacks, FeedbackViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
            //BOOKING
            CreateMap<Bookings, BookingViewModel>()
            .ForMember(dest => dest.CustomerUserName, opt => opt.MapFrom(src => src.customer))
            .ForMember(dest => dest.CustomerFullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.LessonTitle, opt => opt.MapFrom(src => src.Lesson.Title))
            .ForMember(dest => dest.InstructorUserName, opt => opt.MapFrom(src => src.Lesson.Instructor))
            .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            //Roles
            CreateMap<Roles, RoleDTO>().ReverseMap();

            //User
            CreateMap<Users, UserDTO>().ReverseMap();
            CreateMap<UpdateUser, Users>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.DOB, opt => opt.MapFrom(src => src.DOB));

            CreateMap<Users, UserModelDTO>()
                      .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
                      .ReverseMap()
                      .ForMember(dest => dest.Role, opt => opt.Ignore());
            //Upgrade
            CreateMap<UpgradeRequest, UpgraViewModel>().ReverseMap();
            CreateMap<UpgradeRequest, UpgradeRequestDto>()
                    .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));
            CreateMap<Users, UserViewModel>().ReverseMap().ForMember(dest => dest.UserName, otp => otp.MapFrom(src => src.UserName));
            CreateMap<Users, UserViewInstructorModel>()
                .ForMember(dest => dest.LanguageId, opt => opt.MapFrom(src => src.Profile.LanguageId))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Profile.Address))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Profile.Price))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Profile.Country))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Profile.Price))
                .ForMember(dest => dest.TeachingExperience, opt => opt.MapFrom(src => src.Profile.TeachingExperience))
                .ForMember(dest => dest.Education, opt => opt.MapFrom(src => src.Profile.Education));
            CreateMap<RegisterInstructorModel, RegisterBaseModel>().ReverseMap();

            //Map Entity vs DTO ở đây nhé 
            CreateMap<UpdateTutor, Users>()
                .ForPath(dest => dest.Profile.Address, opt => opt.MapFrom(src => src.Address))
                .ForPath(dest => dest.Profile.TeachingExperience, opt => opt.MapFrom(src => src.TeachingExperience))
                .ForPath(dest => dest.Profile.Education, opt => opt.MapFrom(src => src.Education))
                .ForPath(dest => dest.Profile.Price, opt => opt.MapFrom(src => src.Price))
                .ForPath(dest => dest.Profile.Country, opt => opt.MapFrom(src => src.Country));

            // Map từ Certifications sang CertificationDTO
            CreateMap<Certifications, CertificationsDTO>().ReverseMap();

            // Map languages
            CreateMap<Languagues, LanguagesDTO>().ReverseMap();

            //Lesson
            CreateMap<Lessons, CreateLessonModel>().ReverseMap();
            CreateMap<CreateLessonModel, Lessons>()
            .ForMember(dest => dest.LessonId, opt => opt.Ignore());
            CreateMap<Lessons, LessonViewModel>()
                .ForMember(dest => dest.LanguageName, opt => opt.MapFrom(src => src.User.Languagues.FirstOrDefault().LanguageName));

            //TutorAvailabilities
            CreateMap<TutorAvailabilities, CreateTutorAvailabilityModel>().ReverseMap();
            CreateMap<TutorAvailabilities, ScheduleModel>().ReverseMap();

            //Message
            CreateMap<MessageRooms, MessageRoomDTO>()
                        .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Members));
            CreateMap<MessageRoomMember, MessageRoomMemberDTO>();

            // Payments
            CreateMap<Payments, PaymentDTO>();

            //Transaction
            CreateMap<Transactions, TransactionDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Wallet.UserName));

            // Booking
            CreateMap<Bookings, BookingDTO>()
                .ForMember(dest => dest.Instructor, opt => opt.MapFrom(src => src.TutorAvailability.Instructor))
                .ForMember(dest => dest.meetingLink, opt => opt.MapFrom(src => src.TutorAvailability.meetingLink))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Lesson.Title))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.TutorAvailability.StartTime));

            // Feedbacks
            CreateMap<Feedbacks, FeedbacksDTO>();
            CreateMap<CreateFeedback, Feedbacks>();
        }
    }
}

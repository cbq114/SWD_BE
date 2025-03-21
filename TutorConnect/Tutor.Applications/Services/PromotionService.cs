using AutoMapper;
using Tutor.Applications.Interfaces;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.PaymentModel;
using Tutor.Shared.Helper;

namespace Tutor.Applications.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public PromotionService(IPromotionRepository promotionRepository, IUserRepository userRepository, ILessonRepository lessonRepository, IBookingRepository bookingRepository, IMapper mapper)
        {
            _promotionRepository = promotionRepository;
            _userRepository = userRepository;
            _lessonRepository = lessonRepository;
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<string> CreatePromotion(CreatePromotion promo)
        {
            var lesson = await _lessonRepository.GetLessonById(promo.LessonId);
            if (lesson.Status == LessonStatus.Inactive)
                return "Error: This lesson is inactive, please try others lesson";

            var promotion = _mapper.Map<Promotions>(promo);
            var result = await _promotionRepository.CreatePromotion(promotion);
            if (result)
                return "Create successfully";
            return "Error while create promotion, please try later";
        }

        public async Task<string> DeletePromotion(string tutor, int promotionId)
        {
            var promotion = await _promotionRepository.GetById(promotionId);
            if (promotion == null)
                throw new Exception($"Error: Cannot find promotion with id: {promotionId}");

            if (promotion.Instructor != tutor)
                throw new Exception($"You dont have permission to delete this promotion");

            var result = await _promotionRepository.DeletePromotion(promotionId);
            if (result)
                return "Delete promotion success";

            return "Error while delete promotion, please try later";
        }

        public async Task<List<PromotionDTO>> GetAllPromotionOfTutor(string username)
        {
            var tutor = await _userRepository.GetTutor(username);
            if (tutor == null)
                Console.WriteLine($"Cannot find tutor with user name {username} while get promotion");

            var promos = await _promotionRepository.GetAllPromotionOfTutor(username);

            return _mapper.Map<List<PromotionDTO>>(promos);
        }

        public async Task<PromotionDTO> GetByCode(string code, int bookingId)
        {
            var booking = await _bookingRepository.GetBookingById(bookingId);

            if (booking == null)
                throw new Exception($"Cannot find booking with id: {bookingId}");

            var promo = await _promotionRepository.GetByCode(code, booking.TutorAvailability.Instructor);
            if (promo == null)
                Console.WriteLine($"Cannot find promotion with code: {code}");

            return _mapper.Map<PromotionDTO>(promo);
        }

        public async Task<PromotionDTO> GetById(int promotionId)
        {
            var promo = await _promotionRepository.GetById(promotionId);
            if (promo == null)
                Console.WriteLine($"Cannot find promotion with id: {promotionId}");

            return _mapper.Map<PromotionDTO>(promo);
        }

        public async Task<PromotionDTO> UpdatePromotion(PromotionDTO promotion)
        {
            var existingPromotion = await _promotionRepository.GetById(promotion.promotionId);

            if (existingPromotion == null)
                throw new Exception($"Promotion with ID {promotion.promotionId} not found.");

            _mapper.Map(promotion, existingPromotion);
            await _promotionRepository.UpdatePromotion(existingPromotion);
            return _mapper.Map<PromotionDTO>(existingPromotion);
        }

        public async Task<Promotions?> GetByTutorAndCode(string tutor, string code)
        {
            var now = DateTimeHelper.GetVietnamNow().AddHours(7);
            var promotion = await _promotionRepository.GetByTutorAndCode(tutor, code);
            if (promotion == null)
                throw new Exception($"Cannot found promotion of {tutor} with code: {code}");

            if (promotion.Discount <= 0 || promotion.Discount > 100)
                throw new Exception($"Invalid promotion of {tutor} with code {code} to use");

            if (promotion.StartDate > now || promotion.EndDate < now)
                throw new Exception("This promotion is not in valid time to use!");

            if (promotion.limit <= 0 || promotion.Status == PromotionStatus.Inactive)
                throw new Exception("This promotion is out and can not be used");

            return promotion;
        }

        public async Task<bool> CheckDuplicateCodeOfTutor(string tutor, string code)
        {
            var promotion = await _promotionRepository.GetByTutorAndCode(tutor, code);
            if (promotion == null)
                return false;

            return true;
        }
    }
}
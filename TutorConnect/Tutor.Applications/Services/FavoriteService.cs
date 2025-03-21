using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.Applications.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoriteService(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public Task<bool> AddFavoriteInstructor(string userName, string instructorId)
        {
            return _favoriteRepository.AddFavoriteInstructor(userName, instructorId);
        }

        public Task<List<UserViewInstructorModel>> GetFavoriteInstructors(string userName)
        {
            return _favoriteRepository.GetFavoriteInstructors(userName);
        }

        public Task<bool> RemoveFavoriteInstructor(string userName, string instructorId)
        {
            return _favoriteRepository.RemoveFavoriteInstructor(userName, instructorId);
        }
    }
}

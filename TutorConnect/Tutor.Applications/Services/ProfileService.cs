using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Interfaces;

namespace Tutor.Applications.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository repository;

        public ProfileService(IProfileRepository repository)
        {
            this.repository = repository;
        }

        public async Task<List<string>> GetAllCountriesAsync()
        {
            return await repository.GetAllCountriesAsync();
        }
    }
}

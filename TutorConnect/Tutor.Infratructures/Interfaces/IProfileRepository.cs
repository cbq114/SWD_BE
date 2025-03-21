using Tutor.Domains.Entities;

namespace Tutor.Infratructures.Interfaces
{
    public interface IProfileRepository
    {
        Task<List<string>> GetAllCountriesAsync();
        Task<string> GetLanguageOfTutor(string tutor);
        Task<Profile> GetProfileByUsername(string username);
    }
}

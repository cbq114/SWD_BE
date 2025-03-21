using Microsoft.EntityFrameworkCore;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Configurations;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Persistence;

namespace Tutor.Infratructures.Repositories
{
    public class ProfileRepository : Repository<Profile>, IProfileRepository
    {
        public ProfileRepository(TutorDBContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<string>> GetAllCountriesAsync()
        {
            return await Entities.Select(p => p.Country).Distinct().ToListAsync();
        }

        public async Task<string> GetLanguageOfTutor(string tutor)
        {
            var profile = await Entities
                .Include(p => p.Subject)
                .FirstOrDefaultAsync(p => p.UserName == tutor);
            if (profile == null)
                throw new Exception($"Cannot find profile of tutor: {tutor}");

            return profile.Subject.LanguageName;
        }

        public async Task<Profile> GetProfileByUsername(string username)
        {
            return await Entities.FirstOrDefaultAsync(p => p.UserName == username);
        }
    }
}

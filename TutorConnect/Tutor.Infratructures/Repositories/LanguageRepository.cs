using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Configurations;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Persistence;

namespace Tutor.Infratructures.Repositories
{
    public class LanguageRepository : Repository<Languagues>, ILanguageRepository
    {
        private readonly TutorDBContext _dbContext;
        public LanguageRepository(TutorDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Languagues> CreateLanguage(Languagues languague)
        {
            if (languague == null)
                throw new ArgumentNullException(nameof(languague), "Languages cannot be null.");

            await base.Add(languague);
            return languague;
        }

        public async Task<bool> DeleteLanguage(int id)
        {
            var lang = await GetById(id);
            if (lang == null)
                throw new ArgumentNullException(nameof (lang), $"Can not find langues with id = {id}");

            bool isUsed = await _dbContext.Profile.AnyAsync(p => p.LanguageId == id);
            if (isUsed)
                throw new InvalidOperationException($"Cannot delete LanguageId {id} because it is in use.");

            try
            {
                base.Remove(id);
                _dbContext.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new ArgumentNullException("Fail when delete language: ", ex);
            }

        }

        public async Task<List<Languagues>> GetAllLanguages()
        {
            return await Entities.ToListAsync();
        }

        public async Task<Languagues> UpdateLanguage(Languagues language)
        {
            var lang = await Entities.FirstOrDefaultAsync(l => l.LanguageId == language.LanguageId);
            if (lang == null)
                throw new ArgumentNullException(nameof(lang), $"Can not find langues with id = {language.LanguageId}");

            await base.Update(language);
            return language;
        }

        public async Task<Languagues> GetById(int id)
        {
            var lang = await Entities.FirstOrDefaultAsync(l => l.LanguageId == id);
            if (lang == null)
                throw new ArgumentNullException(nameof(lang), $"Can not find langues with id = {id}");
            return lang;
        }
    }
}

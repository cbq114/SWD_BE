using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Domains.Entities;

namespace Tutor.Infratructures.Interfaces
{
    public interface ILanguageRepository
    {
        Task<List<Languagues>> GetAllLanguages();
        Task<Languagues> CreateLanguage(Languagues language);
        Task<Languagues> UpdateLanguage(Languagues language);
        Task<bool> DeleteLanguage(int id);
        Task<Languagues> GetById(int id);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.Applications.Interfaces
{
    public interface ILanguageService
    {
        Task<List<LanguagesDTO>> GetAllLanguages();
        Task<LanguagesDTO> GetLanguageById(int id);
        Task<string> CreateLanguage(LanguagesDTO language);
        Task<string> UpdateLanguage(LanguagesDTO language);
        Task<string> DeleteLanguage(int id);
    }
}

using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Tutor.Applications.Interfaces;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Mapper;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.Applications.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly IMapper _mapper;
        public LanguageService(ILanguageRepository languageRepository, IMapper mapper)
        {
            _languageRepository = languageRepository;
            _mapper = mapper;
        }
        public async Task<string> CreateLanguage(LanguagesDTO language)
        {
            if (language == null)
                return "language cannot be null";

            var lang = _mapper.Map<Languagues>(language);
            var createdLang = await _languageRepository.CreateLanguage(lang);

            if (createdLang == null)
            {
                return "Create language failed!";
            }

            return "Language created successfully";
        }

        public async Task<string> DeleteLanguage(int id)
        {
            var isDeleted = await _languageRepository.DeleteLanguage(id);
            if (!isDeleted)
                return "Delete failed! Language not found or has related data.";

            return "Deleted successfully";

        }

        public async Task<List<LanguagesDTO>> GetAllLanguages()
        {
            var listLang = await _languageRepository.GetAllLanguages();
            return listLang.Select(lang => _mapper.Map<LanguagesDTO>(lang)).ToList();
        }

        public async Task<LanguagesDTO> GetLanguageById(int id)
        {
            var lang = await _languageRepository.GetById(id);
            return _mapper.Map<LanguagesDTO>(lang);
        }

        public async Task<string> UpdateLanguage(LanguagesDTO languageDTO)
        {
            var lang = await _languageRepository.GetById((int) languageDTO.LanguageId);
            if (lang == null)
                return "Language not found!";

            _mapper.Map(languageDTO, lang);
            var updatedLang = await _languageRepository.UpdateLanguage(lang);
            if (updatedLang == null)
                return "Update failed!";

            return "Updated successfully";
        }
    }
}

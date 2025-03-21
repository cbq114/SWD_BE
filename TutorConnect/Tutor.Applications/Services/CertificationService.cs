using AutoMapper;
using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.CertificateModel;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.Applications.Services
{
    public class CertificationService : ICertificationService
    {
        private readonly ICertificationRepository _certificationRepository;
        private readonly IMapper _mapper;
        public CertificationService(ICertificationRepository certificationRepository, IMapper mapper)
        {
            _certificationRepository = certificationRepository;
            _mapper = mapper;
        }
        public Task<string> AddAsync(AddCertification model, string userName)
        {
            return _certificationRepository.AddAsync(model, userName);
        }

        public Task DeleteAsync(int id)
        {
            return _certificationRepository.DeleteAsync(id);
        }

        public Task<IEnumerable<CertificationDto>> GetCerByUsernameAsync1(string username)
        {
            return _certificationRepository.GetCerByUsernameAsync1(username);
        }

        public async Task<List<CertificationsDTO>> GetCerByUsernameAsyncs(string userName)
        {
            var certis = await _certificationRepository.GetCerByUsernameAsync(userName);

            if (certis == null || !certis.Any())
            {
                return new List<CertificationsDTO>();
            }
            return _mapper.Map<List<CertificationsDTO>>(certis);
        }

        public Task<Certifications> UpdateAsync(int id, UpdateCertification model, string userName)
        {
            return _certificationRepository.UpdateAsync(id, model, userName);
        }
    }
}

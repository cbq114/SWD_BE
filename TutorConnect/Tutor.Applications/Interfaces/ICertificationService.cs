using Tutor.Infratructures.Models.CertificateModel;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.Applications.Interfaces
{
    public interface ICertificationService
    {
        Task<List<CertificationsDTO>> GetCerByUsernameAsyncs(string userName);
        Task<Certifications> UpdateAsync(int id, UpdateCertification model, string userName);
        Task DeleteAsync(int id);
        Task<string> AddAsync(AddCertification model, string userName);

        Task<IEnumerable<CertificationDto>> GetCerByUsernameAsync1(string username);

    }
}

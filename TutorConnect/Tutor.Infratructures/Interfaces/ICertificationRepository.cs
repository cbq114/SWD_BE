using Tutor.Infratructures.Models.CertificateModel;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.Infratructures.Interfaces
{
    public interface ICertificationRepository
    {
        Task<string> AddAsync(Models.UserModel.AddCertification certificationModel, string username);
        Task DeleteAsync(int id);
        Task<List<Certifications>> GetCerByUsernameAsync(string username);
        Task<Certifications> GetByIdAndUsernameAsync(int id, string username);
        Task<Certifications> UpdateAsync(int id, UpdateCertification certificationModel, string username);
        Task<IEnumerable<CertificationDto>> GetCerByUsernameAsync1(string username);
    }
}

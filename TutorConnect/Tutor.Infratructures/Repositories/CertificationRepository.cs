using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tutor.Infratructures.Configurations;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.CertificateModel;
using Tutor.Infratructures.Models.UserModel;
using Tutor.Infratructures.Persistence;

namespace Tutor.Infratructures.Repositories
{
    public class CertificationRepository : Repository<Certifications>, ICertificationRepository
    {
        private readonly TutorDBContext _context;
        private readonly IConfiguration _configuration;
        //private readonly IConverter _converter;
        public CertificationRepository(TutorDBContext context, IConfiguration configuration/*, IConverter converter*/) : base(context)
        {
            _context = context;
            _configuration = configuration;
            //_converter = converter;
        }

        public async Task<string> AddAsync(Models.UserModel.AddCertification certificationModel, string username)
        {
            if (certificationModel == null || certificationModel.Certification == null)
                return "Error: Certification model or file cannot be null.";

            var cloudinaryService = new CloundinaryRepository(_configuration);
            var certificationUrl = await cloudinaryService.UploadCertiImage(certificationModel.Certification);
            if (certificationUrl == null)
                return "Error: Failed to upload certification file.";

            var profile = await _context.Profile.FirstOrDefaultAsync(p => p.UserName == username);
            if (profile == null)
                return $"Error: Cannot found profile with username: {username}";

            var certification = new Certifications
            {
                CertificationName = certificationModel.CertificationName,
                CertificationFile = certificationUrl,
                ProfileId = profile.ProfileId,
                Type = "Intructor"
            };
;
            Entities.Add(certification);
            await _context.SaveChangesAsync();

            return "Add certification successfully";
        }

        public async Task DeleteAsync(int id)
        {
            var existingCertification = await Entities.FirstOrDefaultAsync(c => c.CertificationId == id);

            Entities.Remove(existingCertification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Certifications>> GetCerByUsernameAsync(string username)
        {
            var profile = await _context.Profile.FirstOrDefaultAsync(p => p.UserName == username);
            var certification = await Entities.Where(c => c.ProfileId == profile.ProfileId).ToListAsync();
            if (!certification.Any())
            {
                Console.WriteLine(new KeyNotFoundException($"Certification with {username} not found for the user."));
                return null;
            }
            
            return certification;
        }
        public async Task<Certifications> GetByIdAndUsernameAsync(int id, string username)

        {
            var profile = await _context.Profile.FirstOrDefaultAsync(p => p.UserName == username);
            var certification = await Entities.FirstOrDefaultAsync(c => c.CertificationId == id && c.ProfileId == profile.ProfileId);
            if (certification == null)
            {
                Console.WriteLine(new KeyNotFoundException($"Certification with ID {id} not found for the user."));
                return null;
            }
                

            return certification;
        }

        public async Task<Certifications> UpdateAsync(int id, UpdateCertification certificationModel, string username)
        {
            var existingCertification = await GetByIdAndUsernameAsync(id, username);
            if (existingCertification == null || certificationModel == null)
                return null;

            existingCertification.CertificationName = certificationModel.CertificationName;
            if (certificationModel.Certification != null)
            {
                var cloudinaryService = new CloundinaryRepository(_configuration);
                var certificationUrl = await cloudinaryService.UploadCertiImage(certificationModel.Certification);
                if (!string.IsNullOrEmpty(certificationUrl))
                {
                    existingCertification.CertificationFile = certificationUrl;
                }
            }

            await _context.SaveChangesAsync();
            return existingCertification;
        }

        private async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            try
            {
                var cloudinaryService = new CloundinaryRepository(_configuration);
                return await cloudinaryService.UploadCertiImage(file);
            }
            catch (Exception ex)
            {
                throw new Exception("File upload failed.", ex);
            }
        }

        public async Task<IEnumerable<CertificationDto>> GetCerByUsernameAsync1(string username)
        {
            var certifications = await _context.Certifications
                 .Where(c => c.profile.UserName == username)
                 .Select(c => new CertificationDto
                 {
                     CertificationId = c.CertificationId,
                     CertificationName = c.CertificationName,
                     CertificationFile = c.CertificationFile,
                     Type = c.Type
                 })
                 .ToListAsync();

            return certifications;
        }

        //public async Task<byte[]> GenerateCertificateAsync(int courseId, string userName)
        //{
        //    try
        //    {
        //        var user = await _context.Entity.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == userName);
        //        var course = await _context.Course.AsNoTracking().FirstOrDefaultAsync(c => c.CourseId == courseId);
        //        var instructor = await _context.User.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == course.Username);
        //    } catch(Exception ex)
        //    {
        //        throw new Exception();
        //    }
        //}

    }
}

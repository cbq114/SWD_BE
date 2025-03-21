using Microsoft.AspNetCore.Http;

namespace Tutor.Infratructures.Interfaces
{
    public interface ICloundinaryRepository
    {
        Task<string> UploadImage(IFormFile file);
    }
}

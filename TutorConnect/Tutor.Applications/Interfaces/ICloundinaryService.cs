using Microsoft.AspNetCore.Http;

namespace Tutor.Applications.Interfaces
{
    public interface ICloundinaryService
    {
        Task<string> UploadImage(IFormFile file);
    }
}

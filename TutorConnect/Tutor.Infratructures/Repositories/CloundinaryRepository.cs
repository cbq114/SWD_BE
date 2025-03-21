using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;
using Tutor.Infratructures.Interfaces;

namespace Tutor.Infratructures.Repositories
{
    public class CloundinaryRepository : ICloundinaryRepository
    {
        private readonly Cloudinary _cloudinary;
        public CloundinaryRepository(IConfiguration configuration)
        {
            _cloudinary = new Cloudinary(new Account(
                cloud: configuration["Cloudinary:CloudName"],
                apiKey: configuration["Cloudinary:ApiKey"],
                apiSecret: configuration["Cloudinary:ApiSecret"]
            ));
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream())
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception($"Cloudinary upload error: {uploadResult.Error.Message}");
            }

            return uploadResult.SecureUrl?.AbsoluteUri ?? throw new Exception("Image upload failed, SecureUri is null.");
        }

        public async Task<string> UploadCertiImage(IFormFile file)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "certification" 
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception($"Cloudinary upload error: {uploadResult.Error.Message}");
            }

            return uploadResult.SecureUrl?.AbsoluteUri ?? throw new Exception("Image upload failed, SecureUri is null.");
        }


    }
}

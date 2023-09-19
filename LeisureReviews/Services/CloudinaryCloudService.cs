using LeisureReviews.Services.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Net;

namespace LeisureReviews.Services
{
    public class CloudinaryCloudService : ICloudService
    {
        private readonly Cloudinary cloudinary;

        public CloudinaryCloudService(IConfiguration configuration)
        {
            Account account = new Account(
              configuration.GetSection("Cloudinary").GetValue<string>("Cloud"),
              configuration.GetSection("Cloudinary").GetValue<string>("ApiKey"),
              configuration.GetSection("Cloudinary").GetValue<string>("ApiSecret"));
            this.cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadAsync(byte[] content, string extension)
        {
            using (var memoryStream = new MemoryStream(content))
            {
                var name = Guid.NewGuid().ToString();
                var uploadParams = new ImageUploadParams { File = new FileDescription(name, memoryStream), PublicId = name };
                var uploadedResult = await cloudinary.UploadAsync(uploadParams);
                return uploadedResult.PublicId;
            }
        }

        public async Task<byte[]> GetAsync(string fileId)
        {
            var result = await cloudinary.GetResourceAsync(fileId);
            if (result != null && result.StatusCode == HttpStatusCode.OK)
            {
                using (var webClient = new HttpClient())
                    return await webClient.GetByteArrayAsync(result.SecureUrl);
            }
            return default;
        }

        public async Task DeleteAsync(string fileId)
        {
            if (fileId is null) return;
            await cloudinary.DestroyAsync(new DeletionParams(fileId));
        }
    }
}

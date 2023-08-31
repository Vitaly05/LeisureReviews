using Dropbox.Api;
using LeisureReviews.Services.Interfaces;

namespace LeisureReviews.Services
{
    public class DropboxCloudService : ICloudService
    {
        private readonly string accessToken;

        public DropboxCloudService(IConfiguration configuration)
        {
            accessToken = configuration.GetSection("DropboxAccessToken").Value;
        }

        public async Task Upload(byte[] content, string extension)
        {
            using (var dropboxClient = new DropboxClient(accessToken))
            using (var memoryStream = new MemoryStream(content))
            {
                await dropboxClient.Files.UploadAsync($"/{Guid.NewGuid()}{extension}", body: memoryStream);
            }
        }
    }
}

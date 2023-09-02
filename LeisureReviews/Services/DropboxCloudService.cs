using Dropbox.Api;
using LeisureReviews.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace LeisureReviews.Services
{
    public class DropboxCloudService : ICloudService
    {
        private readonly string accessToken;

        public DropboxCloudService(IConfiguration configuration)
        {
            accessToken = configuration.GetSection("DropboxAccessToken").Value;
        }

        public async Task<string> UploadAsync(byte[] content, string extension)
        {
            using (var dropboxClient = new DropboxClient(accessToken))
            using (var memoryStream = new MemoryStream(content))
            {
                var uploaded = await dropboxClient.Files.UploadAsync($"/{Guid.NewGuid()}{extension}", body: memoryStream);
                return uploaded.Id;
            }
        }

        public async Task<byte[]> GetAsync(string fileId)
        {
            using (var dropboxClient = new DropboxClient(accessToken))
            using (var response = await dropboxClient.Files.DownloadAsync(fileId))
            {
                return await response.GetContentAsByteArrayAsync();
            }
        }
    }
}

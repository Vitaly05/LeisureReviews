using Dropbox.Api;
using Dropbox.Api.Files;
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

        public async Task DeleteAsync(string fileId)
        {
            if (fileId is null) return;
            using (var dropboxClient = new DropboxClient(accessToken))
            {
                try
                {
                    await dropboxClient.Files.DeleteV2Async(fileId);
                }
                catch (ApiException<DeleteError> ex)
                {
                    Console.WriteLine($"An error occurred while deleting a file: {ex.Message}");
                }
            }
        }
    }
}

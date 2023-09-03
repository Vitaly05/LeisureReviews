namespace LeisureReviews.Services.Interfaces
{
    public interface ICloudService
    {
        Task<string> UploadAsync(byte[] content, string extension);

        Task<byte[]> GetAsync(string fileId);

        Task DeleteAsync(string fileId);
    }
}

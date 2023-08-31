namespace LeisureReviews.Services.Interfaces
{
    public interface ICloudService
    {
        Task Upload(byte[] content, string extension);
    }
}

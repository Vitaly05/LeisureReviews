using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.Interfaces;
using LeisureReviews.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeisureReviews.Repositories
{
    public class IllustrationsRepository : BaseRepository, IIllustrationsRepository
    {
        private readonly ICloudService cloudService;

        public IllustrationsRepository(ApplicationContext context, ICloudService cloudService) : base(context)
        {
            this.cloudService = cloudService;
        }

        public async Task AddAsync(string reviewId, IFormFile file)
        {
            var illustrationId = await uploadIllustrationAsync(file);
            await context.Illustrations.AddAsync(new Illustration { Id = illustrationId, ReviewId = reviewId });
            await context.SaveChangesAsync();
        }

        public async Task DeleteAllAsync(string reviewId)
        {
            var illustrations = context.Illustrations.Where(i => i.ReviewId == reviewId);
            var removedIds = await illustrations.Select(i => i.Id).ToListAsync();
            context.RemoveRange(await illustrations.ToListAsync());
            await context.SaveChangesAsync();
            foreach (var id in removedIds)
                await cloudService.DeleteAsync(id);
        }

        private async Task<string> uploadIllustrationAsync(IFormFile illustration)
        {
            if (illustration is null) return null;
            using (var reader = new StreamReader(illustration.OpenReadStream()))
            {
                var bytes = default(byte[]);
                using (var memoryStream = new MemoryStream())
                {
                    reader.BaseStream.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
                }
                return await cloudService.UploadAsync(bytes, Path.GetExtension(illustration.FileName));
            }
        }
    }
}

using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeisureReviews.Repositories
{
    public class RatesRepository : BaseRepository, IRatesRepository
    {
        public RatesRepository(ApplicationContext context) : base(context) { }

        public async Task<Rate> GetAsync(User user, Leisure leisure) =>
            await context.Rates.FirstOrDefaultAsync(r => r.User.Id == user.Id && r.Leisure.Id == leisure.Id);

        public async Task<double> GetAverageRateAsync(Leisure leisure)
        {
            IQueryable<Rate> allRates = context.Rates.Where(r => r.Leisure.Id == leisure.Id);
            if (!await allRates.AnyAsync()) return double.NaN;
            return Math.Round(await allRates.AverageAsync(r => r.Value), 1);
        }

        public async Task SaveAsync(Rate rate)
        {
            if (context.Rates.Any(r => r.User.Id == rate.User.Id && r.Leisure.Id == rate.Leisure.Id))
                await updateRateAsync(rate);
            else await context.Rates.AddAsync(rate);
            await context.SaveChangesAsync();
        }

        private async Task updateRateAsync(Rate updatedRate)
        {
            var rate = await GetAsync(updatedRate.User, updatedRate.Leisure);
            rate.Value = updatedRate.Value;
            context.Rates.Update(rate);
        }
    }
}

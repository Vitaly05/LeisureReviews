namespace LeisureReviews.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly ApplicationContext context;

        public BaseRepository(ApplicationContext context)
        {
            this.context = context;
        }
    }
}

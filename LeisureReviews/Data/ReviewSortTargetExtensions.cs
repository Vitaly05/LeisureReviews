namespace LeisureReviews.Data
{
    public static class ReviewSortTargetExtensions
    {
        public static Dictionary<ReviewSortTarget, string> StringValues = new()
        {
            { ReviewSortTarget.Date, "Latest reviews" },
            { ReviewSortTarget.Rate, "Top-rated reviews" },
        };
    }
}

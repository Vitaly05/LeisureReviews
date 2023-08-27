﻿using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.Interfaces;

namespace LeisureReviews.Repositories
{
    public class ReviewsRepository : IReviewsRepository
    {
        private readonly ApplicationContext context;

        public ReviewsRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public void SaveReview(Review review)
        {
            using (context)
            {
                if (context.Reviews.Any(r => r.Id == review.Id))
                {
                    context.Reviews.Update(review);
                    context.Entry(review).Property(r => r.CreateTime).IsModified = false;
                }
                else
                {
                    initReview(review);
                    context.Reviews.Add(review);
                }
                context.SaveChanges();
            }
        }

        private void initReview(Review review)
        {
            review.CreateTime = DateTime.Now;
            review.Id = Guid.NewGuid().ToString();
        }
    }
}

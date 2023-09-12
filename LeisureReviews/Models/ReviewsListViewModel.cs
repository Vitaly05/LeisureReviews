﻿using LeisureReviews.Data;
using LeisureReviews.Models.Database;

namespace LeisureReviews.Models
{
    public class ReviewsListViewModel : PagesViewModel
    {
        public List<Review> Reviews { get; set; }

        public ReviewSortModel ReviewSortModel { get; set; } = new();
    }
}

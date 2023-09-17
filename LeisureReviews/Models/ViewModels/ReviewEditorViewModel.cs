﻿using LeisureReviews.Models.Database;

namespace LeisureReviews.Models.ViewModels
{
    public class ReviewEditorViewModel : BaseViewModel
    {
        public string AuthorName { get; set; }

        public Review Review { get; set; }

        public List<Tag> Tags { get; set; }
    }
}
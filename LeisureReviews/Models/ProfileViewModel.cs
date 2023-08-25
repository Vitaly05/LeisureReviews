﻿using LeisureReviews.Models.Database;

namespace LeisureReviews.Models
{
    public class ProfileViewModel : BaseViewModel
    {
        public User User { get; set; }

        public bool CanEdit
        {
            get => User.UserName == UserName;
        }
    }
}

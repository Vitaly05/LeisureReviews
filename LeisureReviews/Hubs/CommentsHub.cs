using LeisureReviews.Models.Database;
using LeisureReviews.Repositories;
using LeisureReviews.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Globalization;

namespace LeisureReviews.Hubs
{
    public class CommentsHub : Hub
    {
        private readonly IUsersRepository usersRepository;

        private readonly IReviewsRepository reviewsRepository;

        private readonly ICommentsRepository commentsRepository;

        public CommentsHub(IUsersRepository usersRepository, IReviewsRepository reviewsRepository, ICommentsRepository commentsRepository)
        {
            this.usersRepository = usersRepository;
            this.reviewsRepository = reviewsRepository;
            this.commentsRepository = commentsRepository;
        }

        public async Task Init(string reviewId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, reviewId);
        }

        [Authorize]
        public async Task Send(string text, string reviewId)
        {
            var comment = await createCommentAsync(text, reviewId);
            await commentsRepository.SaveAsync(comment);
            await Clients.Group(reviewId).SendAsync("NewComment", comment.Author.UserName, 
                comment.CreateTime.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture), comment.Text);
        }

        private async Task<Comment> createCommentAsync(string text, string reviewId)
        {
            return new Comment()
            {
                Text = text,
                Author = await usersRepository.GetAsync(Context.User),
                Review = await reviewsRepository.GetAsync(reviewId)
            };
        }
    }
}

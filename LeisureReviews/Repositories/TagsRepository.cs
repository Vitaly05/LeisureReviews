﻿using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeisureReviews.Repositories
{
    public class TagsRepository : ITagsRepository
    {
        private readonly ApplicationContext context;

        public TagsRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<List<Tag>> GetTagsAsync() =>
            await context.Tags.ToListAsync();

        public async Task<ICollection<Tag>> GetTagsAsync(IEnumerable<string> tagsNames)
        {
            List<Tag> tags = new List<Tag>();
            foreach (var tagName in tagsNames)
                tags.Add(await context.Tags.FirstOrDefaultAsync(t => t.Name == tagName));
            return tags;
        }

        public void AddNewTags(IEnumerable<string> tagsNames)
        {
            foreach (var tagName in tagsNames)
                if (!context.Tags.Any(t => t.Name == tagName))
                    context.Tags.Add(new Tag() { Id = Guid.NewGuid().ToString(), Name = tagName });
            context.SaveChanges();
        }
    }
}
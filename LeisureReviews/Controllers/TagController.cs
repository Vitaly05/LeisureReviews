using LeisureReviews.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LeisureReviews.Controllers
{
    public class TagController : Controller
    {
        private readonly ITagsRepository tagsRepository;

        public TagController(ITagsRepository tagsRepository)
        {
            this.tagsRepository = tagsRepository;
        }

        public async Task<IActionResult> GetTagsWeights()
        {
            return Ok(await tagsRepository.GetWeightsAsync());
        }
    }
}

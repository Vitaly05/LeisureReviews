using LeisureReviews.Models;
using Microsoft.AspNetCore.Mvc;

namespace LeisureReviews.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var model = new BaseViewModel();
            model.IsAuthorized = HttpContext.User.Identity.IsAuthenticated;
            model.UserName = HttpContext.User.Identity.Name;
            return View(model);
        }
    }
}
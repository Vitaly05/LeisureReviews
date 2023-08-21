using Microsoft.AspNetCore.Mvc;

namespace LeisureReviews.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
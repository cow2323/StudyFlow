using Microsoft.AspNetCore.Mvc;

namespace StudyFlow.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace StudyFlow.Controllers
{
    public class StudySessionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

    }
}

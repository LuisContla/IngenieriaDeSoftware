using Microsoft.AspNetCore.Mvc;

namespace Práctica_1.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Books()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View();
        }

    }
}

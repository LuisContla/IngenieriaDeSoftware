using Microsoft.AspNetCore.Mvc;

namespace Tarea_3.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

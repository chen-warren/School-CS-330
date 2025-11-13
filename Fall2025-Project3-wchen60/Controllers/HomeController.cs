using Microsoft.AspNetCore.Mvc;

namespace Fall2025_Project3_wchen60.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
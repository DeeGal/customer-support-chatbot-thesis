using Microsoft.AspNetCore.Mvc;

namespace CloverBotWebApplication.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();   
        }
    }
}

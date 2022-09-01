using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SuperMarket_Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize]
    public class HomeController : Controller
    {
        //[Authorize(Roles ="Admin")]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Report()
        {
            return View();
        }
    }
}

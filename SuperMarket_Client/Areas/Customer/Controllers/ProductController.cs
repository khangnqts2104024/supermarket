using Microsoft.AspNetCore.Mvc;

namespace SuperMarket_Client.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }
    }
}

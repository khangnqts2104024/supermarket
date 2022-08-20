using Microsoft.AspNetCore.Mvc;

namespace SuperMarket_Client.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

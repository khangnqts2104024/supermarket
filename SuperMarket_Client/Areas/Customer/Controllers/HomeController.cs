using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;

namespace SuperMarket_Client.Areas.Customer.Controllers
{
        [Area("Customer")]
        public class HomeController : Controller
        {
            private readonly IUnitOfWork unitOfWork;


            public HomeController(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<IActionResult> Index()
            {
                try
                {
                    HttpContext.Session.SetInt32("branchId", 1);
                var data = await unitOfWork.Product.GetAll(includeProperties: "ImageProduct,Brand_Category.Category");
                var a = await unitOfWork.Category.GetAll();
                ViewBag.CategoryList = await unitOfWork.Category.GetAll();
                return View(data);

                }
                catch (Exception)
                {

                    return View();
                }

            }

            public IActionResult Privacy()
            {
                return View();
            }

        [HttpGet]
        public IActionResult CartListViewComponent()
        {
            return ViewComponent("CartList");
        }

        
    }
    
}

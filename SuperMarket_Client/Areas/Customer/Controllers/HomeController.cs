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
                var getSS = HttpContext.Session.GetInt32("branchId");
                var data = await unitOfWork.Product.GetAll(includeProperties: "ImageProduct,Brand_Category.Category");
                ViewBag.CategoryList = await unitOfWork.Category.GetAll();
                ViewBag.getSS = getSS;
                return View(data);
            }
            catch (Exception)
            {

                return ViewBag.Error="Error";

            }

        }
        public IActionResult CreateSession(int selectBranch)
        {
            HttpContext.Session.SetInt32("branchId", selectBranch);
            return RedirectToAction("Index");

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


using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;

namespace SuperMarket_Client.Areas.Customer.Controllers
{
    [Area("Customer")]
    [BranchActionFilter]
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
                var branchId = int.Parse(HttpContext.Request.Cookies["branchId"]);
                if(branchId == null)
                {
                    var data = await unitOfWork.Product.GetAll(includeProperties: "ImageProduct,Brand_Category.Category");
                    ViewBag.CategoryList = await unitOfWork.Category.GetAll();
                    return View(data);
                }
                else
                {
                    var stockList = await unitOfWork.Stock.GetAll(x=>x.BranchId == branchId && x.Count >0,includeProperties: "Product.Brand_Category.Category,Product.ImageProduct");
                    ViewBag.CategoryList = await unitOfWork.Category.GetAll();
                    return View(stockList);
                }
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Error"); 

            }


        }
        public async Task<IActionResult> CreateCookieForBrachId(int selectBranch)
        {

            var branch = await unitOfWork.Branch.GetFirstOrDefault(x=>x.BranchId == selectBranch);

            if(branch != null)
            {
                CookieOptions cookieOptions = new CookieOptions() {
                    Expires = DateTime.Now.AddDays(30),
                };
                Response.Cookies.Append("branchId", selectBranch.ToString(),cookieOptions);
                Response.Cookies.Append("branchName", branch.BranchName, cookieOptions);
            }

            return RedirectToAction("Index", new {id=selectBranch});
        }


      


        [HttpGet]
        public IActionResult CartListViewComponent()
        {
            return ViewComponent("CartList");
        }
        public IActionResult AboutUs()
        {
            return View();
        }

    }

}

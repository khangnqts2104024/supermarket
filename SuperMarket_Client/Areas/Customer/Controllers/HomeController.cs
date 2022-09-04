
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;
using SuperMarket_Utility;

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
        public async Task<IActionResult> Index(int? id)
        {
            try
            {

                if(id == null)
                {
                    var data = await unitOfWork.Product.GetAll(includeProperties: "ImageProduct,Brand_Category.Category");
                    ViewBag.CategoryList = await unitOfWork.Category.GetAll();
                    return View(data);
                }
                else
                {
                    var finalData = new List<Product>();
                    var data=await unitOfWork.Stock.GetAll(x=>x.BranchId==id);
                    ViewBag.CategoryList = await unitOfWork.Category.GetAll();

                    foreach (var item in data)
                    {
                        var temp = await unitOfWork.Product.GetAll(x => x.ProductId == item.ProductId, includeProperties: "ImageProduct,Brand_Category.Category");
                        finalData.AddRange(temp);
                    }
                    return View(finalData);
                }
            }
            catch (Exception)
            {

                return NotFound();

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

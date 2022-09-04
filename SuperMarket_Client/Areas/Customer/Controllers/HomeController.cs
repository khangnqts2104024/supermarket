
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using SuperMarket_DataAccess.Repository.IRepository;
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
        public async Task<IActionResult> Index()
        {
            try
            {
               var id = HttpContext.Request.Cookies["branchId"];
                var data = await unitOfWork.Product.GetAll(includeProperties: "ImageProduct,Brand_Category.Category");
                ViewBag.CategoryList = await unitOfWork.Category.GetAll();
                return View(data);
            }
            catch (Exception)
            {

                return ViewBag.Error="Error";

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

            return RedirectToAction("Index");
        }


      


        [HttpGet]
        public IActionResult CartListViewComponent()
        {
            return ViewComponent("CartList");
        }


    }

}

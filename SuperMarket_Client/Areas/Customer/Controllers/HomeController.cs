
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;
using SuperMarket_Models.ViewModels;

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
                ProductController productController = new ProductController(unitOfWork);
                if (id == null)
                {
                    var branchId = int.TryParse(HttpContext.Request.Cookies["branchId"], out int result);
                    var stockList = await unitOfWork.Stock.GetAll(x => x.BranchId == result && x.Count > 0, includeProperties: "Product.Brand_Category.Category,Product.ImageProduct");
                    var ratingList = await unitOfWork.Feedback_Rating.GetAll();
                    if(ratingList != null)
                    {
                        foreach (var item in stockList)
                        {
                            item.RatingPointAverage = productController.CalculateRatingPointAverage((List<Feedback_Rating>)ratingList, item.Product.ProductId);
                        }
                    }
                    
                    ViewBag.CategoryList = await unitOfWork.Category.GetAll();
                    return View(stockList);
                }
                else
                {
                    var stockList = await unitOfWork.Stock.GetAll(x => x.BranchId == id && x.Count > 0, includeProperties: "Product.Brand_Category.Category,Product.ImageProduct");
                    var ratingList = await unitOfWork.Feedback_Rating.GetAll();
                    if (ratingList != null)
                    {
                        foreach (var item in stockList)
                        {
                            item.RatingPointAverage = productController.CalculateRatingPointAverage((List<Feedback_Rating>)ratingList, item.Product.ProductId);
                        }
                    }
                    ViewBag.CategoryList = await unitOfWork.Category.GetAll();
                    return View(stockList);
                }
                
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Error" ,new { area = "Customer" }); 

            }


        }
        public async Task<IActionResult> CreateCookieForBrachId(int selectBranch)
        {
            try
            {
                var branch = await unitOfWork.Branch.GetFirstOrDefault(x => x.BranchId == selectBranch);

                if (branch != null)
                {
                    CookieOptions cookieOptions = new CookieOptions()
                    {
                        Expires = DateTime.Now.AddDays(30),
                    };
                    Response.Cookies.Append("branchId", selectBranch.ToString(), cookieOptions);
                    Response.Cookies.Append("branchName", branch.BranchName, cookieOptions);
                }

                return RedirectToAction("Index", new { id = selectBranch });
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Error", new { area = "Customer" });

            }

        }


      


        [HttpGet]
        public IActionResult CartListViewComponent()
        {
            try
            {
                return ViewComponent("CartList");

            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Error", new { area = "Customer" });

            }
        }
        public IActionResult AboutUs()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Error", new { area = "Customer" });

            }
        }

    }

}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;
using SuperMarket_Models.ViewModels;
using System.Security.Claims;

namespace SuperMarket_Client.Areas.Customer.Controllers
{
    [Area("Customer")]
    [BranchActionFilter]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var data = await unitOfWork.Product.GetAll(includeProperties: "Brand_Category,Brand_Category.Brand,Brand_Category.Category");
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {

            int? branchId = HttpContext.Session.GetInt32("branchId");
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (id == 0 || branchId == null)
            {
                return RedirectToAction("Index", "Home", new { Area = "Customer" });
            }
         
            Cart_Feedback_RatingVM objVM = new Cart_Feedback_RatingVM()
            {
               branchId = (int)branchId,
               Feedback_RatingList = (List<Feedback_Rating>)await unitOfWork.Feedback_Rating.GetAll(x=>x.ProductId == id,includeProperties:"Product,Customer"),
               Product = await unitOfWork.Product.GetFirstOrDefault(x => x.ProductId == id, includeProperties: "Brand_Category.Brand,Brand_Category.Category,Stock.Branch"),
               Count = 1,
               ProductId = id,
            };
            objVM.Feedback_RatingList = objVM.Feedback_RatingList.OrderByDescending(x=>x.Id).ToList();


            objVM.RatingPointAverage = CalculateRatingPointAverage(objVM.Feedback_RatingList, id);


            if (objVM.Product != null)
            {
                var stockByBranchID = objVM.Product.Stock.FirstOrDefault(x => x.BranchId == branchId && x.ProductId == id);
                objVM.StockCount = (int)stockByBranchID.Count;
            }
            objVM.FeedbackCount = objVM.Feedback_RatingList.Count();


            return View(objVM);
        }
      
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(Cart_Feedback_RatingVM objVM)
        {
            if (User.Identity != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var claimsIdentity = (ClaimsIdentity)User.Identity;
                    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    if (claim != null)
                    {
                        var shoppingCart = new ShoppingCart()
                        {
                            Count = objVM.Count,
                            ProductId = objVM.ProductId,
                            CustomerId = claim.Value
                        };
                        var cartFromDb = await unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.CustomerId == claim.Value && x.ProductId == objVM.ProductId);
                        if (cartFromDb != null)
                        {

                            var stock = await unitOfWork.Stock.GetFirstOrDefault(x => x.BranchId == objVM.branchId && x.ProductId == objVM.ProductId);

                            if (objVM.Count + cartFromDb.Count > stock.Count)
                            {
                                return Json(new
                                {
                                    statusCode = 400,
                                    message = "The product you have selected has reached a limited quantity",
                                    count = 1,
                                });
                            }

                            unitOfWork.ShoppingCart.IncrementCount(cartFromDb, objVM.Count);
                            await unitOfWork.Save();
                            return Json(new
                            {
                                statusCode = 200,
                                message = "Updated Cart Successfully",
                                count = 1,
                            });
                        }
                        else
                        {

                            var stock = await unitOfWork.Stock.GetFirstOrDefault(x => x.BranchId == objVM.branchId && x.ProductId == objVM.ProductId);

                            if (objVM.Count > stock.Count)
                            {
                                return Json(new
                                {
                                    statusCode = 400,
                                    message = "The product you have selected has reached a limited quantity",
                                    count = 1,
                                });
                            }
                           
                            await unitOfWork.ShoppingCart.Add(shoppingCart);

                            await unitOfWork.Save();
                            return Json(new
                            {
                                statusCode = 201,
                                message = "Added To Cart Successfully",
                                count = 1,
                            });
                        }
                    }
                }
            }
            return Json(new
            {
                statusCode = 401,
                message = "User Not Authenticated",
            });
        }

        
        public  int CalculateRatingPointAverage(List<Feedback_Rating> ratingListByProductId,int productId)
        {
            var result = ratingListByProductId.Where(x => x.ProductId == productId)
                .GroupBy(x => x.RatingPoint).Select(
                g => new
                {
                    RatingPoint = g.Key,
                    Count = g.Count()
                }
                ).ToList();

            int numOf1Star = 0;
            int numOf2Star = 0;
            int numOf3Star = 0;
            int numOf4Star = 0;
            var numOf5Star = 0;

            if (result.FirstOrDefault(x => x.RatingPoint == 1) != null)
            {
                numOf1Star = result.FirstOrDefault(x => x.RatingPoint == 1).Count;
            }
            if (result.FirstOrDefault(x => x.RatingPoint == 2) != null)
            {
                numOf2Star = result.FirstOrDefault(x => x.RatingPoint == 2).Count;
            }
            if (result.FirstOrDefault(x => x.RatingPoint == 3) != null)
            {
                numOf3Star = result.FirstOrDefault(x => x.RatingPoint == 3).Count;
            }
            if (result.FirstOrDefault(x => x.RatingPoint == 4) != null)
            {
                numOf4Star = result.FirstOrDefault(x => x.RatingPoint == 4).Count;
            }
            if (result.FirstOrDefault(x => x.RatingPoint == 5) != null)
            {
                numOf5Star = result.FirstOrDefault(x => x.RatingPoint == 5).Count;
            }
            int totalNumberOfRating = ratingListByProductId.Count();
            if(totalNumberOfRating == 0)
            {
                totalNumberOfRating = 1;
            }
            
            return (int)(1 * numOf1Star + 2 * numOf2Star + 3 * numOf3Star + 4 * numOf4Star + 5 * numOf5Star) / totalNumberOfRating;
        }    

   
        //khang ss/


        public async Task<IActionResult> CompareProduct(int? CategoryId, int? productId)
        {
            if (CategoryId == null) { CategoryId = 1; }
            
           
            var ListCategory = await unitOfWork.Category.GetAll();
                   ViewBag.listCate = new SelectList(ListCategory, "CategoryId", "CategoryName");
        
                var cate= await unitOfWork.Category.GetFirstOrDefault(c => c.CategoryId.Equals(CategoryId));
                 ViewBag.CateName = cate.CategoryName;
         var product = await unitOfWork.Product.GetFirstOrDefault(p => p.ProductId.Equals(productId), includeProperties: "Brand_Category.Brand,ImageProduct");

            if (product != null)
            {
                ViewBag.dProduct = product;
            }
            else { ViewBag.dProduct = null; }
                

            var model = await unitOfWork.Product.GetAll(p => p.Brand_Category.CategoryId.Equals(CategoryId), includeProperties: "Brand_Category.Brand,ImageProduct");

            return View(model);
        }












    }
}

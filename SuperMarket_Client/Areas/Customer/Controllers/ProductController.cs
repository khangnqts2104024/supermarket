using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;
using SuperMarket_Models.ViewModels;
using System.Linq;
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
        public async Task<IActionResult> Index(int? cateID)
        {
            try
            {
                var branchId = int.TryParse(HttpContext.Request.Cookies["branchId"], out int result);
                IEnumerable<Stock> data;
                if (result != 0)
                {
                    if (cateID != null)
                    {
                        data = await unitOfWork.Stock.GetAll(s => s.BranchId.Equals(result) && s.Product.Brand_Category.CategoryId.Equals(cateID), includeProperties: "Product.Brand_Category,Product.ImageProduct,Product.Brand_Category.Brand,Product.Brand_Category.Category,Product.Feedback_Ratings");
                    }
                    else
                    {
                        data = await unitOfWork.Stock.GetAll(s => s.BranchId.Equals(result), includeProperties: "Product.Brand_Category,Product.ImageProduct,Product.Brand_Category.Brand,Product.Brand_Category.Category,Product.Feedback_Ratings");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                var categoryList = await unitOfWork.Category.GetAll();
                var brandList = await unitOfWork.Brand.GetAll();
                ViewBag.categoryList = categoryList;
                ViewBag.brandList = brandList;

                return View(data);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error", new {area = "Customer"});
            }
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var branchId = int.TryParse(HttpContext.Request.Cookies["branchId"], out int result);
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (id == 0 || result == 0)
                {
                    return RedirectToAction("Index", "Home", new { Area = "Customer" });
                }
                Cart_Feedback_RatingVM objVM = new Cart_Feedback_RatingVM()
                {
                    branchId = result,
                    Feedback_RatingList = (List<Feedback_Rating>)await unitOfWork.Feedback_Rating.GetAll(x => x.ProductId == id, includeProperties: "Product,Customer"),
                    Product = await unitOfWork.Product.GetFirstOrDefault(x => x.ProductId == id, includeProperties: "Brand_Category.Brand,Brand_Category.Category,Stock.Branch,ImageProduct"),
                    Count = 1,
                    ProductId = id,
                };
                objVM.Feedback_RatingList = objVM.Feedback_RatingList.OrderByDescending(x => x.Id).ToList();
                objVM.RatingPointAverage = CalculateRatingPointAverage(objVM.Feedback_RatingList, id);
                if (objVM.Product != null)
                {
                    var stockByBranchID = objVM.Product.Stock.FirstOrDefault(x => x.BranchId == result && x.ProductId == id);
                    objVM.StockCount = (int)stockByBranchID.Count;
                }
                objVM.FeedbackCount = objVM.Feedback_RatingList.Count();

                bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
                if (isAjax)
                {
                    return Json(new
                    {
                        branchId = result,
                        Feedback_RatingList = objVM.Feedback_RatingList,
                        Product = objVM.Product,
                        Count = 1,
                        ProductId = objVM.ProductId,
                        FeedbackCount = objVM.FeedbackCount,
                        RatingPointAverage = objVM.RatingPointAverage
                    });
                }
                var ratingList = await unitOfWork.Feedback_Rating.GetAll();
                if(claim != null)
                {
                    var userReviewed = await unitOfWork.Feedback_Rating.GetFirstOrDefault(x => x.CustomerId == claim.Value && x.ProductId == id);
                    if (userReviewed != null)
                    {
                        objVM.usedToReview = true;
                        objVM.UserReview_RatingPoint = userReviewed.RatingPoint;
                        objVM.UserReview_Content = userReviewed.Content;
                    }
                    else
                    {
                        objVM.usedToReview = false;
                        objVM.UserReview_RatingPoint = 0;
                        objVM.UserReview_Content = "";

                    }
                }
                var relatedProduct = await unitOfWork.Stock.GetAll(x => x.Product.Brand_Category.CategoryId == objVM.Product.Brand_Category.CategoryId && x.BranchId == result && x.ProductId != objVM.ProductId, includeProperties: "Product.Brand_Category.Category,Product.ImageProduct");
                if (ratingList != null)
                {
                    foreach (var item in relatedProduct)
                    {
                        item.RatingPointAverage = CalculateRatingPointAverage((List<Feedback_Rating>)ratingList, item.Product.ProductId);
                    }
                }
                 objVM.RelatedProduct = relatedProduct.Take(7).ToList();
                return View(objVM);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error", new { area = "Customer" });
            }

           

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(Cart_Feedback_RatingVM objVM)
        {
            try
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
            catch (Exception)
            {
                return Json(new
                {
                    statusCode = 500,
                    message = "Something went wrong...",
                });
            }
        }
        public int CalculateRatingPointAverage(List<Feedback_Rating> ratingListByProductId, int productId)
        {
            try
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
                if (totalNumberOfRating == 0)
                {
                    totalNumberOfRating = 1;
                }

                return (int)(1 * numOf1Star + 2 * numOf2Star + 3 * numOf3Star + 4 * numOf4Star + 5 * numOf5Star) / totalNumberOfRating;
            }
            catch (Exception)
            {

                return 0;
            }
        }
        //khang ss/
        //${item.Brand_Category.CategoryId}
        public async Task<IActionResult> CompareProduct(int? CategoryId, int? productId)
        {
            try
            {
                if (CategoryId == null) { CategoryId = 1; }
                var ListCategory = await unitOfWork.Category.GetAll();
                ViewBag.listCate = new SelectList(ListCategory, "CategoryId", "CategoryName");
                var cate = await unitOfWork.Category.GetFirstOrDefault(c => c.CategoryId.Equals(CategoryId));
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
            catch (Exception)
            {
                return RedirectToAction("Index", "Error", new { area = "Customer" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> SearchByName(string? search)
        {
            try
            {
                var branchId = int.TryParse(HttpContext.Request.Cookies["branchId"], out int result);
                IEnumerable<Stock> data;
                if (result != 0 && search != null)
                {
                    data = await unitOfWork.Stock.GetAll(s => s.BranchId.Equals(result) && s.Product.ProductName.ToLower().Contains(search.ToLower()), includeProperties: "Product.Brand_Category,Product.ImageProduct,Product.Brand_Category.Brand,Product.Brand_Category.Category,Product.Feedback_Ratings");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                var categoryList = await unitOfWork.Category.GetAll();
                var brandList = await unitOfWork.Brand.GetAll();
                ViewBag.categoryList = categoryList;
                ViewBag.brandList = brandList;

                return View("Index", data);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error", new { area = "Customer" });
            }
        }
    }
}

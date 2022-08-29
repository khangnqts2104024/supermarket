using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;
using System.Security.Claims;

namespace SuperMarket_Client.Areas.Customer.Controllers
{
    [Area("Customer")]
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
            if (id == 0)
            {
                return RedirectToAction("Index", "Home", new { Area = "Customer" });
            }
            ShoppingCart cartObj = new ShoppingCart()
            {
                Count = 1,
                ProductId = id,
                Product = await unitOfWork.Product.GetFirstOrDefault(x => x.ProductId == id, includeProperties: "Brand_Category", thenIncludeProperties: "Brand,Category"),

            };
            return View(cartObj);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            if (User.Identity != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var claimsIdentity = (ClaimsIdentity)User.Identity;
                    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    if (claim != null)
                    {
                        shoppingCart.CustomerId = claim.Value;
                        var cartFromDb = await unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.CustomerId == claim.Value && x.ProductId == shoppingCart.ProductId);
                        if (cartFromDb != null)
                        {
                            unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
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

        //khang ss/


        public async Task<IActionResult> CompareProduct()
        {
            var model = await unitOfWork.Product.GetAll(p => p.Brand_Category.CategoryId.Equals(1), includeProperties: "Brand_Category.Brand");

            return View(model);
        }












    }
}

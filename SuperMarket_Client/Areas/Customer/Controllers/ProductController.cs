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
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int productId)
        {
            productId = 2;
            ShoppingCart cartObj = new ShoppingCart()
            {
                Count = 1,
                ProductId = productId,
                Product = await unitOfWork.Product.GetFirstOrDefault(x => x.ProductId == productId, includeProperties: "Brand_Category", thenIncludeProperties: "Brand,Category"),
            };
            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddToCart(ShoppingCart shoppingCart)
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
                        if(cartFromDb != null)
                        {
                            unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
                        }
                        else
                        {
                           await unitOfWork.ShoppingCart.Add(shoppingCart);
                        }
                       await unitOfWork.Save();
                    }
                }
            }

            return RedirectToAction("Index");
        }
    }
}

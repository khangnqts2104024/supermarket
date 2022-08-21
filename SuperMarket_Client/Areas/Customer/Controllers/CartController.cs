using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;
using SuperMarket_Models.ViewModels;
using System.Security.Claims;

namespace SuperMarket_Client.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CartController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            if(User.Identity != null)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = await unitOfWork.ShoppingCart.GetAll(x => x.CustomerId == claim.Value, includeProperties: "Product"),
                    Order = new()
                };

                foreach (var item in shoppingCartVM.ListCart)
                {
                    shoppingCartVM.Order.OrderTotal += (item.Product.Price * item.Count);
                }
                return View(shoppingCartVM);
            }
            return View();
        }

        public async Task<IActionResult> CheckOut()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return View();
        }

        public async Task<IActionResult> CompleteOrder()
        {
            return View();
        }

        public async Task<IActionResult> Plus()
        {
            return View();
        }

        public async Task<IActionResult> Minus()
        {
            return View();
        }

    }
}

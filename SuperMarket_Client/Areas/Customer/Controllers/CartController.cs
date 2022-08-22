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

        [HttpGet]
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

        //public async Task<IActionResult> Checkout()
        //{
        //    var claimsIdentity = (ClaimsIdentity)User.Identity;
        //    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        //    return View();
        //}

        public async Task<IActionResult> CompleteOrder()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Plus(int cartId)
        {
            var cartFromDb = await unitOfWork.ShoppingCart.GetFirstOrDefault(x=>x.CartId == cartId,includeProperties:"Product");
            if(cartFromDb != null)
            {
                unitOfWork.ShoppingCart.IncrementCount(cartFromDb, 1);
                await unitOfWork.Save();

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
                    return Json(new
                    {
                        statusCode = 200,
                        message = "Increment Count Successfully",
                        count = cartFromDb.Count,
                        subTotalItem = cartFromDb.Product.Price * cartFromDb.Count,
                        subTotalOrder = shoppingCartVM.Order.OrderTotal
                    });
            }
            else
            {
                return Json(new
                {
                    statusCode = 404,
                    message = "Cannot Found Item"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartId)
        {
            var cartFromDb = await unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.CartId == cartId, includeProperties: "Product");
            if (cartFromDb != null)
            {
                    unitOfWork.ShoppingCart.Remove(cartFromDb);
                    await unitOfWork.Save();
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

                return Json(new
                    {
                        statusCode = 200,
                        message = "Remove Success!",
                        actionClient = "removed",
                        subTotalOrder = shoppingCartVM.Order.OrderTotal
                });
            }
            else
            {
                return Json(new
                {
                    statusCode = 404,
                    message = "Cannot Found Item"
                });
            }
        }



        [HttpPost]
        public async Task<IActionResult> Minus(int cartId, string? actionClient)
        {
            var cartFromDb = await unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.CartId == cartId, includeProperties: "Product");
            if (cartFromDb != null)
            {
                if (cartFromDb.Count <= 1 && string.IsNullOrEmpty(actionClient))
                {
                    return Json(new
                    {
                        statusCode = 200,
                        message = "Do you want to remove item from cart?",
                        actionClient = "ask"
                    });

                }
                else if(cartFromDb.Count <= 1 && actionClient == "confirmed")
                {
                    unitOfWork.ShoppingCart.Remove(cartFromDb);
                    await unitOfWork.Save();
                    return Json(new
                    {
                        statusCode = 200,
                        message = "Remove Success!",
                        actionClient = "removed"
                    });
                }
                else
                {
                    unitOfWork.ShoppingCart.DecrementCount(cartFromDb, 1);
                    await unitOfWork.Save();
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
                    return Json(new
                    {
                        statusCode = 200,
                        message = "Decrement Count Successfully",
                        count = cartFromDb.Count,
                        subTotalItem = cartFromDb.Product.Price * cartFromDb.Count,
                        subTotalOrder = shoppingCartVM.Order.OrderTotal
                    });

                }
            }
            else
            {
                return Json(new
                {
                    statusCode = 404,
                    message = "Cannot Found Item"
                });
            }
        }

    }
}

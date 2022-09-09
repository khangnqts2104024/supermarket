using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;
using System.Security.Claims;

namespace SuperMarket_Client.ViewComponents
{
    [ViewComponent(Name = "CartList")]
    public class CartListViewComponent : ViewComponent
    {
        private readonly IUnitOfWork unitOfWork;

        public CartListViewComponent(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                List<ShoppingCart> cartList = (List<ShoppingCart>)await unitOfWork.ShoppingCart.GetAll(x => x.CustomerId == claim.Value, includeProperties: "Product.ImageProduct");
                decimal totalCart = 0;
                if (cartList.Count() == 0)
                {
                    ViewBag.CartCount = 0;
                    ViewBag.totalCart = 0;
                    return View("CartList", cartList);
                }

                foreach (var item in cartList)
                {
                    totalCart += item.Product.Price * item.Count;
                }

                ViewBag.CartCount = cartList.Count();
                ViewBag.cartId = cartList.FirstOrDefault().CartId;
                ViewBag.totalCart = totalCart;
                return View("CartList", cartList);
            }
            catch (Exception)
            {

                throw;
            }
        }

        
    }
}

using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;

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
            var cartList = await unitOfWork.ShoppingCart.GetAll(includeProperties:"Product");
            decimal totalCart = 0;

            foreach (var item in cartList)
            {
                totalCart += item.Product.Price * item.Count;
            }

            ViewBag.CartCount = cartList.Count();
            ViewBag.totalCart = totalCart;



            return View("CartList", cartList);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;
using SuperMarket_Models.ViewModels;
using SuperMarket_Utility;
using System.Security.Claims;

namespace SuperMarket_Client.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CouponController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        
   

        public CouponController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> AddCoupon(string couponCode)
        {
        //check coupon

            var coupon =await unitOfWork.Coupon.GetFirstOrDefault(c => c.CouponCode.Equals(couponCode));
            if (coupon != null)
            {
                if (coupon.ExpiredDate > DateTime.Now)
                {
                    if(coupon.Count > 0)
                    {
                        var discountPercent = coupon.DiscountPercent;
                        var orderTotalBeforeCoupon = await GetOrderTotal();
                        var orderTotalAfterCoupon = orderTotalBeforeCoupon * (100 - discountPercent) / 100;
                        var discountAmount = orderTotalBeforeCoupon - orderTotalAfterCoupon;
                            return Json(new
                            {
                            statusCode = 200,
                            cpCode = coupon.CouponCode,
                            message = "Applied Coupon Successfully",
                            orderTotalBeforeCoupon = orderTotalBeforeCoupon,
                            orderTotalAfterCoupon = orderTotalAfterCoupon,
                            discountAmount = discountAmount,
                            couponId = coupon.CouponId
                            });
                    }
                    else
                    {
                        return Json(new
                        {
                            statusCode = 200,
                            cpCode = SD.CouponExpired
                        });
                    }

                }
                else
                {
                    return Json(new
                    {
                        statusCode = 200,
                        cpCode = "Expired"
                    });
                }

            }
            else 
            {
                return Json(new
                {
                    statusCode = 200,
                    cpCode = SD.CouponNotExists
                });
            }
        }

        public async Task<decimal> GetOrderTotal()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
            {
                ListCart = (List<ShoppingCart>)await unitOfWork.ShoppingCart.GetAll(x => x.CustomerId == claim.Value, includeProperties: "Product"),
                Order = new(),
                //khang

            };

            foreach (var item in shoppingCartVM.ListCart)
            {
                shoppingCartVM.Order.OrderTotal += (item.Product.Price * item.Count);
            }
            return shoppingCartVM.Order.OrderTotal;
        }

       
    }
}

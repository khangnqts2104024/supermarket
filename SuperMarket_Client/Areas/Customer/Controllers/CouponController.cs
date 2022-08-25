using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;

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

        public async Task<IActionResult> AddCoupon(string couponCode)
        {
        
            var coupon =await unitOfWork.Coupon.GetFirstOrDefault(c => c.CouponCode.Equals(couponCode));
            if (coupon != null && coupon.Count > 0)
            {

                return RedirectToAction("Checkout", "Cart", new { cpCode = coupon.CouponCode });
            }
            else 
            {
              
                return RedirectToAction("Checkout", "Cart", new { cpCode = "Expired" });

            }


        }
    }
}

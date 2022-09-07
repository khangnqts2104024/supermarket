using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;

namespace SuperMarket_Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CouponController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

   
        public CouponController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork=unitOfWork;
        }

        [HttpGet]
        public IActionResult NewCoupon()
        {
            return  View();
        }
        [HttpPost]
        public async Task<IActionResult> NewCoupon(Coupon newCoupon)
        {
            if (ModelState.IsValid)
            {
                var coupon = await unitOfWork.Coupon.GetFirstOrDefault(c => c.CouponCode.Equals(newCoupon.CouponCode));
                if (coupon == null)
                {
                    newCoupon.CreatedDate = DateTime.Now;
                    await unitOfWork.Coupon.Add(newCoupon);
                    await unitOfWork.Save();

                    return RedirectToAction("CouponManage");
                }
                else

                {

                    return View();

                }
            }
            else
            {
                return View();
            }


         


           
        }

        public async Task<IActionResult> CouponManage() 
        {

            var model = await unitOfWork.Coupon.GetAll();




            return View(model);
        }
    }
}

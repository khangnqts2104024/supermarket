using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;
using System.Security.Claims;

namespace SuperMarket_Client.Areas.Customer.Controllers
{
    [Area("Customer")]
    [BranchActionFilter]

    public class FeedbackAndRatingController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public FeedbackAndRatingController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReview(int productId, string content, int ratingPoint)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claimsIdentity == null || claim == null)
            {
                return Json(new
                {
                    statusCode = 401,
                    message = "Please Login to Review",
                });
            }
            int? branchId = int.Parse(HttpContext.Request.Cookies["branchId"]);
            if(branchId != 0)
            {
                var product = await unitOfWork.Product.GetFirstOrDefault(x => x.ProductId == productId);
                if (product != null)
                {
                    if(!string.IsNullOrEmpty(content))
                    {
                        var feedback = new Feedback_Rating()
                        {
                            Content = content,
                            RatingPoint = ratingPoint,
                            CustomerId = claim.Value,
                            ProductId = productId,
                        };
                        await unitOfWork.Feedback_Rating.Add(feedback);
                        await unitOfWork.Save();

                        return Json(new
                        {
                            statusCode = 200,
                            message = "Send Feedback successfully",
                            content = await unitOfWork.Feedback_Rating.GetFirstOrDefault(x => x.Id == feedback.Id, includeProperties: "Customer")
                        });
                    }
                    return Json(new
                    {
                        statusCode = 400,
                        message = "Bad Request",
                    });


                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            
            
        }
    }
}

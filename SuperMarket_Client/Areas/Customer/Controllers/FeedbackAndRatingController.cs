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
            try
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
                var branchId = int.TryParse(HttpContext.Request.Cookies["branchId"], out int result);
                if (result != 0)
                {
                    var product = await unitOfWork.Product.GetFirstOrDefault(x => x.ProductId == productId);
                    if (product != null)
                    {
                        if (!string.IsNullOrEmpty(content))
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
                                feedbackId = feedback.Id,
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
                        return RedirectToAction("Index", "Error", new
                        {
                            area = "Customer"
                        });
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error", new
                {
                    area = "Customer"
                });
            }           

        }

        //[HttpGet]
        public async Task<IActionResult> EditReview(int newRatingPoint, string newFeedback, int ProductId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

          if(claim != null)
            {
                if (newRatingPoint != 0 && !string.IsNullOrEmpty(newFeedback))
                {
                    var review = await unitOfWork.Feedback_Rating.GetFirstOrDefault(x => x.ProductId == ProductId && x.CustomerId == claim.Value);
                    if(review != null)
                    {
                        review.RatingPoint = newRatingPoint;
                        review.Content = newFeedback;
                        unitOfWork.Feedback_Rating.Update(review);
                        await unitOfWork.Save();
                        return RedirectToAction("Details", "Product", new { area = "Customer" });
                    }
                }
            }
            return RedirectToAction("Details", "Product", new { area = "Customer" });
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Utility;
using System.Security.Claims;

namespace SuperMarket_Client.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]

    public class CustomerController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment env;

        public CustomerController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            this.unitOfWork = unitOfWork;
            this.env = env;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claimsIdentity == null || claim == null)
                {
                    return RedirectToAction("Index", "Home", new { Area = "Customer" });
                }
                else
                {
                    var data = await unitOfWork.Customer.GetFirstOrDefault(x => x.Id == claim.Value);
                    return View(data);
                }
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Error", new { area = "Customer" });

            }


        }
        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(SuperMarket_Models.Models.Customer customer, IFormFile? file)
        {
            try
            {
                string wwwRootPath = env.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"Images\CustomerAvatar");
                    var extension = Path.GetExtension(file.FileName);

                    if (customer.CustomerAvatar != null)
                    {
                        var oldImgPath = Path.Combine(wwwRootPath, customer.CustomerAvatar.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    customer.CustomerAvatar = @"\Images\CustomerAvatar\" + fileName + extension;
                    unitOfWork.Customer.Update(customer);
                    await unitOfWork.Save();
                    return RedirectToAction("Index", new { id = customer.Id });
                }
                else
                {
                    unitOfWork.Customer.Update(customer);
                    await unitOfWork.Save();
                    return RedirectToAction("Index", new { id = customer.Id });
                }
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Error", new { area = "Customer" });

            }

        }

        [HttpGet]
        public async Task<IActionResult> Order()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error", new { area = "Customer" });

            }
        }

        

       
        [HttpGet]
        public async Task<IActionResult> CancelRequest(int orderId)
        {
            try
            {
                if (orderId != 0)
                {
                    var order = await unitOfWork.Order.GetFirstOrDefault(x => x.OrderId == orderId && x.OrderStatus == SD.StatusApproved);
                    if (order != null)
                    {
                        unitOfWork.Order.UpdateStatus(orderId, SD.StatusCancelRequest);
                        await unitOfWork.Save();
                        TempData["MsgCancelOrder"] = "Cancel Request Has Been Sent!";
                        return RedirectToAction("Order", "Customer");
                    }
                    else
                    {
                        TempData["MsgCancelOrder"] = "Can't cancel the order because the order has been completed";
                        return RedirectToAction("Order", "Customer");
                    }
                }

                return RedirectToAction("Order", "Customer");
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Error", new { area = "Customer" });

            }


        }

        #region For datatable Order Customer api call
        [HttpGet]
        public async Task<IActionResult> GetAllOrder()
        {
            try
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var orderList = await unitOfWork.Order.GetAll(x => x.CustomerId == claim.Value);

                foreach (var item in orderList)
                {
                    item.OrderDetail = (List<SuperMarket_Models.Models.OrderDetail>)await unitOfWork.OrderDetail.GetAll(x => x.OrderId == item.OrderId, includeProperties: "Product");
                }

                return Json(new
                {
                    data = orderList,
                });
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Error", new { area = "Customer" });

            }

        }
        #endregion

        

    }
}

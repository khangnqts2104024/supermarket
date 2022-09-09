using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;
using SuperMarket_Utility;

namespace SuperMarket_Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork unitOfWork;


        public OrderController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index(string? status)
        {
        

            try
            {
                IEnumerable<Order> model;

                if (status == null) { model = await unitOfWork.Order.GetAll(includeProperties: "Customer"); }
                else if (status != "other") { model = await unitOfWork.Order.GetAll(o => o.OrderStatus.Equals(status), includeProperties: "Customer"); }
                else { model = await unitOfWork.Order.GetAll(o => o.OrderStatus.Equals(SD.StatusPending) || o.OrderStatus.Equals(SD.StatusCompleted) || o.OrderStatus.Equals(SD.StatusRefunded), includeProperties: "Customer"); }


                return View(model);
            }
            catch (Exception)
            {

                return RedirectToAction("ErrorPage", "Home", new { area = "Admin" });
            }


        }
        [HttpGet]
        public async Task<IActionResult> OrderDetails(int OrderId)
        {

          

            try
            {
                var order = await unitOfWork.Order.GetFirstOrDefault(o => o.OrderId.Equals(OrderId), includeProperties: "Customer,Branch");
                order.OrderDetail = (List<SuperMarket_Models.Models.OrderDetail>)await unitOfWork.OrderDetail.GetAll(od => od.OrderId.Equals(OrderId), includeProperties: "Product");

                return View(order);
            }
            catch (Exception)
            {

                return RedirectToAction("ErrorPage", "Home", new { area = "Admin" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateOrderStatus(int OrderId, string orderStatus)
        {
        

            try
            {
                var order = await unitOfWork.Order.GetFirstOrDefault(o => o.OrderId.Equals(OrderId), includeProperties: "Customer,Branch");
                order.OrderDetail = (List<SuperMarket_Models.Models.OrderDetail>)await unitOfWork.OrderDetail.GetAll(od => od.OrderId.Equals(OrderId), includeProperties: "Product");
                ViewBag.error = "";

                if (order.OrderStatus == SD.StatusApproved && orderStatus == SD.StatusApproved)
                {
                    //minus stock
                    foreach (var item in order.OrderDetail)
                    {
                        var stock = await unitOfWork.Stock.GetFirstOrDefault(s => s.BranchId.Equals(order.BranchId) && s.ProductId.Equals(item.ProductId));
                        if (stock.Count >= item.Count)
                        {
                            unitOfWork.Stock.DecrementStock(stock, item.Count);
                            ViewBag.notice = "Order Processing!";
                        }
                        else
                        {
                            ViewBag.error = item.Product.ProductName + " :Is Out Of Stock in this Branch!Add more stock or contact with customer!";

                            return View("OrderDetails", order);
                        }

                    }
                    //test xem co cần save() trong loop ko?


                    unitOfWork.Order.UpdateStatus(OrderId, SD.StatusInProcess);

                    await unitOfWork.Save();


                    return View("OrderDetails", order);
                }
                else if (order.OrderStatus == SD.StatusInProcess && orderStatus == SD.StatusInProcess)
                {
                    unitOfWork.Order.UpdateStatus(OrderId, SD.StatusCompleted);
                    await unitOfWork.Save();
                    ViewBag.notice = "Congratulation!You Completed the Order!";
                    return View("OrderDetails", order);
                }
                else if (order.OrderStatus == SD.StatusCancelRequest && orderStatus == SD.StatusCancelRequest)
                {
                    unitOfWork.Order.UpdateStatus(OrderId, SD.StatusRefunded);
                    //stock back
                    //foreach (var item in order.OrderDetail)
                    //{
                    //    var stock = await unitOfWork.Stock.GetFirstOrDefault(s => s.BranchId.Equals(order.BranchId) && s.ProductId.Equals(item.ProductId));

                    //    unitOfWork.Stock.IncrementStock(stock, item.Count);
                    //}
                    //await unitOfWork.Save();
                    ViewBag.notice = "Confirm Refund Success!";
                    return View("OrderDetails", order);
                }
                else
                {
                    return RedirectToAction("Index");
                }

            }
            catch (Exception)
            {

                return RedirectToAction("ErrorPage", "Home", new { area = "Admin" });
            }
        }

        
    }
}

using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Utility;

namespace SuperMarket_Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork unitOfWork;


        public OrderController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {

            var model = await unitOfWork.Order.GetAll(includeProperties:"Customer");
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> OrderDetails(int OrderId)
        {

            var order = await unitOfWork.Order.GetFirstOrDefault(o => o.OrderId.Equals(OrderId),includeProperties:"Customer");
            order.OrderDetail =(List<SuperMarket_Models.Models.OrderDetail>)await unitOfWork.OrderDetail.GetAll(od => od.OrderId.Equals(OrderId),includeProperties:"Product");

            return  View(order);
        }

        //[HttpGet]
        //public  async Task<IActionResult> UpdateOrderStatus(int OrderId,string orderStatus)
        //{
        //    var order = await unitOfWork.Order.GetFirstOrDefault(o => o.OrderId.Equals(OrderId));

        //    if (order.OrderStatus==SD.StatusApproved) { }
        //         unitOfWork.Order.UpdateStatus(OrderId, orderStatus);

        //return 
        //}

    }
}

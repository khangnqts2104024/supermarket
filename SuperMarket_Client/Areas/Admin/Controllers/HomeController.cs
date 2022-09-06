using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;
using SuperMarket_Utility;

namespace SuperMarket_Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        //[Authorize(Roles ="Admin")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult OtherPages() { 
        
        
        return View();
        
        }
        public async Task<IActionResult> Report()
        {
           
        



        //string year = "2022"; can select year for report **** optional
    
          
            var orderC= await unitOfWork.Order.GetAll(o=>o.OrderStatus.Equals(SD.StatusCompleted));
            var orderR = await unitOfWork.Order.GetAll(o => o.OrderStatus.Equals(SD.StatusRefunded));
            var orders = await unitOfWork.Order.GetAll(o => o.OrderDate < DateTime.Parse("01-01-2023") && o.OrderDate >= DateTime.Parse("01-01-2022"));
            //chart 1(completed/refunded)
            List<int> ratioReport = new List<int>();
            var completed = orderC.Count(o => o.OrderDate < DateTime.Parse("01-01-2023") && o.OrderDate >= DateTime.Parse("01-01-2022"));
            var refunded = orderR.Count(o => o.OrderDate < DateTime.Parse("01-01-2023") && o.OrderDate >= DateTime.Parse("01-01-2022"));
            ratioReport.Add(completed);
            ratioReport.Add(refunded);
            ratioReport.Add(orders.Count()-completed-refunded);
            ViewBag.ratioReport = ratioReport;


            //mm-dd/yyyy
            //chart2 (orders)
            List<int> orderReport = new List<int>();
            orderReport.Add(orderC.Count(o=>o.OrderDate < DateTime.Parse("04-01-2022") && o.OrderDate >= DateTime.Parse("01-01-2022")));
            orderReport.Add(orderC.Count(o => o.OrderDate < DateTime.Parse("07-01-2022") && o.OrderDate >= DateTime.Parse("04-01-2022")));
            orderReport.Add(orderC.Count(o => o.OrderDate < DateTime.Parse("10-01-2022") && o.OrderDate >= DateTime.Parse("07-01-2022")));
            orderReport.Add(orderC.Count(o => o.OrderDate < DateTime.Parse("01-01-2023") && o.OrderDate >= DateTime.Parse("10-01-2022")));
            ViewBag.OrderReport = orderReport;

            //chart 3(Gross revenue)

            List<decimal> revenueReport =new List<decimal>();
            revenueReport.Add(getRevenue(orderC.Where(o => o.OrderDate < DateTime.Parse("02-01-2022") && o.OrderDate >= DateTime.Parse("01-01-2022") )));
            revenueReport.Add(getRevenue(orderC.Where(o => o.OrderDate < DateTime.Parse("03-01-2022") && o.OrderDate >= DateTime.Parse("02-01-2022"))));
            revenueReport.Add(getRevenue(orderC.Where(o => o.OrderDate < DateTime.Parse("04-01-2022") && o.OrderDate >= DateTime.Parse("03-01-2022"))));
            revenueReport.Add(getRevenue(orderC.Where(o => o.OrderDate < DateTime.Parse("05-01-2022") && o.OrderDate >= DateTime.Parse("04-01-2022"))));
            revenueReport.Add(getRevenue(orderC.Where(o => o.OrderDate < DateTime.Parse("06-01-2022") && o.OrderDate >= DateTime.Parse("05-01-2022"))));
            revenueReport.Add(getRevenue(orderC.Where(o => o.OrderDate < DateTime.Parse("07-01-2022") && o.OrderDate >= DateTime.Parse("06-01-2022"))));
            revenueReport.Add(getRevenue(orderC.Where(o => o.OrderDate < DateTime.Parse("08-01-2022") && o.OrderDate >= DateTime.Parse("07-01-2022"))));
            revenueReport.Add(getRevenue(orderC.Where(o => o.OrderDate < DateTime.Parse("09-01-2022") && o.OrderDate >= DateTime.Parse("08-01-2022"))));
            revenueReport.Add(getRevenue(orderC.Where(o => o.OrderDate < DateTime.Parse("10-01-2022") && o.OrderDate >= DateTime.Parse("09-01-2022"))));
            revenueReport.Add(getRevenue(orderC.Where(o => o.OrderDate < DateTime.Parse("11-01-2022") && o.OrderDate >= DateTime.Parse("10-01-2022"))));
            revenueReport.Add(getRevenue(orderC.Where(o => o.OrderDate < DateTime.Parse("12-01-2022") && o.OrderDate >= DateTime.Parse("11-01-2022"))));
            revenueReport.Add(getRevenue(orderC.Where(o => o.OrderDate < DateTime.Parse("01-01-2023") && o.OrderDate >= DateTime.Parse("12-01-2022"))));
            ViewBag.revenueReport = revenueReport;



            return View();


        }

        public decimal getRevenue(IEnumerable<Order> orders) {
            decimal total=0;
            foreach (var item in orders)
            {
                total += item.OrderTotal;
            }
            return total;

        }

    }
}

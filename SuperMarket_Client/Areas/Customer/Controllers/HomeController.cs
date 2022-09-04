
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Utility;

namespace SuperMarket_Client.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;


        public HomeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var data = await unitOfWork.Product.GetAll(includeProperties: "ImageProduct,Brand_Category.Category");
                ViewBag.CategoryList = await unitOfWork.Category.GetAll();
                return View(data);
            }
            catch (Exception)
            {

                return ViewBag.Error="Error";

            }


        }
        public async Task<IActionResult> CreateSession(int selectBranch)
        {

            var branch = await unitOfWork.Branch.GetFirstOrDefault(x=>x.BranchId == selectBranch);
            if(branch != null)
            {
                HttpContext.Session.SetInt32("branchId", selectBranch);
                HttpContext.Session.SetString("branchName", branch.BranchName);
            }

            return RedirectToAction("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(string fullname, string email, string phone, string message)
        {
            string html = "<h3>" + fullname + "</h3>" +
                "<p>" + email + "</p>" +
                "<p>Phone: " + phone + "</p>" +
                "<p> Message: " + message + "</p>";

            string subject = "Customer have a question!";
            EmailSender emailSender = new EmailSender();
            await emailSender.UserSendEmailAsync(subject, html);
          
            return View();
        }


        [HttpGet]
        public IActionResult CartListViewComponent()
        {
            return ViewComponent("CartList");
        }


    }

}

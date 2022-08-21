using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;
using System.Security.Claims;

namespace SuperMarket_Client.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class CustomerController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CustomerController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        
        public IActionResult Index(string id)
        {
            if(id == null)
            {
                return RedirectToAction("Index","Customer", new { Area = "Customer" });
            }
            else
            {
                var data = unitOfWork.Customer.GetFirstOrDefault(x=>x.Id==id);
                return View(data);
            }
            
        }
    }
}

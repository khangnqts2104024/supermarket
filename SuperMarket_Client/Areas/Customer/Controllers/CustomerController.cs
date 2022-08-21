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
        
        public async Task<IActionResult> Index(string id)
        {
            if(id == null)
            {
                return RedirectToAction("Index","Customer", new { Area = "Customer" });
            }
            else
            {
                var data =await unitOfWork.Customer.GetFirstOrDefault(x=>x.Id==id);
                return View(data);
            }
            
        }
		[HttpPost]
        public async Task<IActionResult> UpdateCustomer(SuperMarket_Models.Models.Customer customer ,IFormFile? file)
        {
			try
			{
                if(file != null)
				{
                    string path = Path.Combine("wwwroot/Images", file.FileName);
                    var stream = new FileStream(path, FileMode.Create);
                    file.CopyToAsync(stream);
                    //customer.CustomerAvatar = "Images/" + file.FileName;
                    unitOfWork.Customer.Update(customer);

                }
			}
			catch (Exception)
			{

				throw;
			}
            return View();

        }
    }
}

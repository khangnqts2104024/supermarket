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
        private readonly IWebHostEnvironment env;

        public CustomerController(IUnitOfWork unitOfWork,IWebHostEnvironment env)
        {
            this.unitOfWork = unitOfWork;
            this.env = env;
        }

        public async Task<IActionResult> Index(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Customer", new { Area = "Customer" });
            }
            else
            {
                var data = await unitOfWork.Customer.GetFirstOrDefault(x => x.Id == id);
                return View(data);
            }

        }
        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(SuperMarket_Models.Models.Customer customer, IFormFile? file)
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
    }
}

using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;

namespace SuperMarket_Client.Areas.Home.Controllers
{
    [Area("Home")]
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

                return View(data);

            }
            catch (Exception)
            {

                return View();
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SuperMarket_DataAccess.Repository.IRepository;

namespace SuperMarket_Client.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class Brand_CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public Brand_CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Brand_CategoryManage()
        {
            var data = await unitOfWork.Brand_Category.GetAll(includeProperties: "Brand,Category");
            var categoryList=await unitOfWork.Category.GetAll();
            var product = await unitOfWork.Product.GetAll();
            ViewBag.product=product;
            ViewBag.CategoryList =categoryList;
            return View(data);
        }
    }
}

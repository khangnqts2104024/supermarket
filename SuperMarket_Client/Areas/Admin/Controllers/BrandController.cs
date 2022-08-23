using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;

namespace SuperMarket_Client.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class BrandController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public BrandController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> CreateBrand()
        {
            var data =await unitOfWork.Category.GetAll();
            ViewBag.categoryList = new SelectList(data,"CategoryId","CategoryName");
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateBrand(SuperMarket_Models.Models.Brand obj)
        {
            if (obj != null)
            {
                var data = await unitOfWork.Brand.GetAll();
                int count = 0;
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (item.BrandName.Contains(obj.BrandName))
                        {
                            count++;
                        }

                    }
                    if (count > 0)
                    {
                        ViewBag.msg = "This brand has been Used. Try another.";
                    }
                    else
                    {
                        await unitOfWork.Brand.Add(obj);
                        await unitOfWork.Save();
                        ViewBag.msg = "Brand has been Created.";
                    }
                }
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> UpdateBrand(int id)
        {
            var data = await unitOfWork.Category.GetFirstOrDefault(x => x.CategoryId == id);
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(SuperMarket_Models.Models.Category obj)
        {
            var data = await unitOfWork.Category.GetAll();
            foreach (var item in data)
            {
                if (item.CategoryName.Contains(obj.CategoryName))
                {
                    ViewBag.msg = "Categories has been Used. Try another.";
                    return View();
                }
                else
                {
                    obj.UpdateDate = DateTime.Now;
                    unitOfWork.Category.Update(obj);
                    await unitOfWork.Save();
                    ViewBag.msg = "Categories has been Updated.";
                }
            }
            return View();
        }
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var data = await unitOfWork.Brand_Category.GetFirstOrDefault(x => x.CategoryId == id);
            if (data != null)
            {
                ViewBag.msg = "This Category has brand and product in it, can not delete unless delete all the brand and product.";
            }
            else
            {
                var data1 = await unitOfWork.Category.GetFirstOrDefault(x => x.CategoryId == id);
                unitOfWork.Category.Remove(data1);
                await unitOfWork.Save();
                ViewBag.msg = "Category has beed deleted";

            }
            return View();
        }
    }
}

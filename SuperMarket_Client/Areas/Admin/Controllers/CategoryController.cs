using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_DataAccess.Services;

namespace SuperMarket_Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> CreateCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory(SuperMarket_Models.Models.Category obj)
        {
            if (obj != null)
            {
                var data = await unitOfWork.Category.GetAll();
                int count = 0;
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (item.CategoryName==obj.CategoryName)
                        {
                            count++;
                        }

                    }
                    if (count > 0)
                    {
                        ViewBag.msg = "Categories has been Used. Try another.";
                    }
                    else
                    {
                        await unitOfWork.Category.Add(obj);
                        await unitOfWork.Save();
                        ViewBag.msg = "Categories has been Created.";
                    }
                }
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id)
        {
            var data = await unitOfWork.Category.GetFirstOrDefault(x => x.CategoryId == id);
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(SuperMarket_Models.Models.Category obj)
        {
            var data = await unitOfWork.Category.GetAll(x=>x.CategoryId!=obj.CategoryId);
            foreach (var item in data)
            {
                if (item.CategoryName.Contains(obj.CategoryName))
                {
                    ViewBag.msg = "Category name has been Used. Try another.";
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
            var data = await unitOfWork.Brand_Category.GetAll(x => x.CategoryId == id);

            foreach (var item in data)
            {
                var temp = await unitOfWork.Product.GetFirstOrDefault(x => x.BrandCateId == item.BrandCateId);
                if (temp != null)
                {
                    ViewBag.msg = "This Category has product in it, can not delete unless delete all product.";
                    return RedirectToAction("Index");
                }
            }
            var data1 = await unitOfWork.Category.GetFirstOrDefault(x => x.CategoryId == id);
            unitOfWork.Category.Remove(data1);
            await unitOfWork.Save();
            ViewBag.msg = "Category has beed deleted";
            return View();
        }
    }
}

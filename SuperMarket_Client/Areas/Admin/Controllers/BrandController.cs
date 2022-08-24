using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;
using System.Collections.Generic;

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
        public async Task<IActionResult> CreateBrand(string message)
        {
            if (message == null)
            {
                var data = await unitOfWork.Category.GetAll();
                ViewBag.categoryList = new SelectList(data, "CategoryId", "CategoryName");
                return View();
            }
            else
            {
                var data = await unitOfWork.Category.GetAll();
                ViewBag.categoryList = new SelectList(data, "CategoryId", "CategoryName");
                ViewBag.msg = message;
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateBrand(Brand obj, int[] CategoryId)
        {
            if (obj != null)
            {
                var data = await unitOfWork.Brand.GetAll(x => x.BrandName.Contains(obj.BrandName));
                int count = 0;
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        item.BrandName.Contains(obj.BrandName);
                        count++;
                    }
                    if (count > 0)
                    {
                        ViewBag.msg = "This brand has been Used. Try another.";
                        return View();
                    }

                    else
                    {
                        await unitOfWork.Brand.Add(obj);
                        await unitOfWork.Save();
                        List<Brand_Category> brand_catelist = new List<Brand_Category>();
                        var checkBrandId = await unitOfWork.Brand.GetFirstOrDefault(x => x.BrandName == obj.BrandName);
                        foreach (var item in CategoryId)
                        {
                            var brand_Cate = new Brand_Category();
                            brand_Cate.BrandId = checkBrandId.BrandId;
                            brand_Cate.CategoryId = item;
                            brand_catelist.Add(brand_Cate);
                        }
                        TempData["brand_catelist"] = JsonConvert.SerializeObject(brand_catelist);
                        return RedirectToAction("AddBrandCategory", "Brand");
                    }
                }
            }
            return null;
        }
        public async Task<IActionResult> AddBrandCategory()
        {
            var brand_catelist = JsonConvert.DeserializeObject<List<Brand_Category>>(TempData["brand_catelist"].ToString());
            await unitOfWork.Brand_Category.AddRange(brand_catelist);
            await unitOfWork.Save();
            return RedirectToAction("CreateBrand", "Brand", new { message = "Brand has been Created." });
        }
        [HttpGet]
        public async Task<IActionResult> UpdateBrand(int id)
        {
            var data = await unitOfWork.Brand.GetFirstOrDefault(x => x.BrandId == id);
            var data1 = await unitOfWork.Brand_Category.GetFirstOrDefault(x => x.BrandId == id);
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBrand(Category obj)
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
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var data = await unitOfWork.Brand_Category.GetAll(x => x.BrandId == id);

            foreach (var item in data)
            {
                var temp = await unitOfWork.Product.GetFirstOrDefault(x => x.BrandCateId == item.BrandCateId);
                if (temp != null)
                {
                    ViewBag.msg = "This Brand has product in it, can not delete unless delete all product.";
                    return RedirectToAction("Index");
                }
            }
            var data1 = await unitOfWork.Brand.GetFirstOrDefault(x => x.BrandId == id);
            unitOfWork.Brand.Remove(data1);
            await unitOfWork.Save();
            ViewBag.msg = "Brand has beed deleted";
            return View();
        }
    }
}

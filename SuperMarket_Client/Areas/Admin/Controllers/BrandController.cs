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
        public async Task<IActionResult> GetAllBrand()
        {
            var data = await unitOfWork.Brand_Category.GetAll(includeProperties:"Brand,Category");

            return Json(new { data = data });
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
                        if (item.BrandName.ToLower() == obj.BrandName.ToLower())
                        {
                            count++;
                        }
                    }
                    if (count > 0)
                    {
                        return RedirectToAction("CreateBrand", "Brand", new { message = "This brand has been Used. Try another." });

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
        public async Task<IActionResult> AddBrandCategory(int? id)
        {
            if (id == null)
            {
                var brand_catelist = JsonConvert.DeserializeObject<List<Brand_Category>>(TempData["brand_catelist"].ToString());
                await unitOfWork.Brand_Category.AddRange(brand_catelist);
                await unitOfWork.Save();
                return RedirectToAction("CreateBrand", "Brand", new { message = "Brand has been Created." });
            }
            else
            {
                var brand_catelist = JsonConvert.DeserializeObject<List<Brand_Category>>(TempData["update_Brand"].ToString());
                await unitOfWork.Brand_Category.AddRange(brand_catelist);
                await unitOfWork.Save();
                return RedirectToAction("UpdateBrand", "Brand", new { message = "Brand has been Updated.", id = id });
            }

        }
        [HttpGet]
        public async Task<IActionResult> UpdateBrand(int id, string? msg)
        {
            var data = await unitOfWork.Brand.GetFirstOrDefault(x => x.BrandId == id);
            var categoryList = await unitOfWork.Category.GetAll();
            var categoryOfBrand = await unitOfWork.Brand_Category.GetAll(x => x.BrandId == id, includeProperties: "Category");
            var chosenCategory = new List<Category>();
            var categoryLeft = new List<Category>();
            foreach (var item in categoryList)
            {
                int count = 0;
                foreach (var chosen in categoryOfBrand)
                {
                    if (item.CategoryId == chosen.Category.CategoryId)
                    {
                        count++;
                    }
                }
                if (count == 0)
                {
                    chosenCategory.Add(item);
                }
                else
                {
                    categoryLeft.Add(item);
                }
            }
            if (msg != null)
            {
                ViewBag.msg = msg;
            }

            ViewBag.categoryLeft = new SelectList(chosenCategory, "CategoryId", "CategoryName");
            ViewBag.chosenCategory = new List<Category>(categoryLeft);
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBrand(Brand obj, int[] CategoryId)
        {
            if (CategoryId.Count() == 0)
            {
                var data = await unitOfWork.Brand.GetAll(x => x.BrandId != obj.BrandId);
                if (data.Count() == 0)
                {
                    obj.UpdateDate = DateTime.Now;
                    unitOfWork.Brand.Update(obj);
                    await unitOfWork.Save();
                    return RedirectToAction("UpdateBrand", new { msg = "Categories has been Updated." });
                }
                foreach (var item in data)
                {
                    if (item.BrandName.ToLower()==obj.BrandName.ToLower())
                    {
                        return RedirectToAction("UpdateBrand", new { msg = "Brand name has been Used. Try another." });
                    }
                    else
                    {
                        obj.UpdateDate = DateTime.Now;
                        unitOfWork.Brand.Update(obj);
                        await unitOfWork.Save();
                        return RedirectToAction("UpdateBrand", new { msg = "Categories has been Updated." });

                    }
                }
            }
            else
            {
                var data = await unitOfWork.Brand.GetAll(x => x.BrandId != obj.BrandId);
                if (data.Count() == 0)
                {
                    List<Brand_Category> brand_Categories = new List<Brand_Category>();

                    obj.UpdateDate = DateTime.Now;
                    unitOfWork.Brand.Update(obj);
                    await unitOfWork.Save();
                    foreach (var temp in CategoryId)
                    {
                        var brand_Cate = new Brand_Category();
                        brand_Cate.BrandId = obj.BrandId;
                        brand_Cate.CategoryId = temp;
                        brand_Categories.Add(brand_Cate);
                    }
                    TempData["update_Brand"] = JsonConvert.SerializeObject(brand_Categories);
                    return RedirectToAction("AddBrandCategory", "Brand", new { id = obj.BrandId });
                }
                else
                {
                    foreach (var item in data)
                    {
                        if (item.BrandName.ToLower()==obj.BrandName.ToLower())
                        {
                            return RedirectToAction("UpdateBrand", new { msg = "Brand name has been Used. Try another." });
                        }
                        else
                        {
                            List<Brand_Category> brand_Categories = new List<Brand_Category>();

                            obj.UpdateDate = DateTime.Now;
                            unitOfWork.Brand.Update(obj);
                            await unitOfWork.Save();
                            foreach (var temp in CategoryId)
                            {
                                var brand_Cate = new Brand_Category();
                                brand_Cate.BrandId = obj.BrandId;
                                brand_Cate.CategoryId = temp;
                                brand_Categories.Add(brand_Cate);
                            }
                            TempData["update_Brand"] = JsonConvert.SerializeObject(brand_Categories);
                            return RedirectToAction("AddBrandCategory", "Brand", new { id = obj.BrandId });
                        }
                    }
                }

            }
            return null;
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var data = await unitOfWork.Brand_Category.GetAll(x => x.BrandId == id);

            foreach (var item in data)
            {
                var temp = await unitOfWork.Product.GetFirstOrDefault(x => x.BrandCateId == item.BrandCateId);
                if (temp != null)
                {
                    return Json(new { success = false, msgFail = "This Brand has product in it, can not delete unless delete all product." });
                }
            }
            var data1 = await unitOfWork.Brand.GetFirstOrDefault(x => x.BrandId == id);
            unitOfWork.Brand.Remove(data1);
            await unitOfWork.Save();
            return Json(new { success = true, msgSuccess = "Brand has beed deleted." });
        }
    }
}

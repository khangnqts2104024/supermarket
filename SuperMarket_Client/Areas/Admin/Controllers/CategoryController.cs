using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_DataAccess.Services;

namespace SuperMarket_Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment env;

        public CategoryController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            this.unitOfWork = unitOfWork;
            this.env = env;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory(SuperMarket_Models.Models.Category obj, IFormFile CategoryImg)
        {
            string wwwRootPath = env.WebRootPath;

            if (obj != null)
            {
                var data = await unitOfWork.Category.GetAll();
                int count = 0;
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (item.CategoryName.ToLower() == obj.CategoryName.ToLower())
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
                        string fileName = Guid.NewGuid().ToString();
                        var uploads = Path.Combine(wwwRootPath, @"Images\CategoryImage");
                        var extension = Path.GetExtension(CategoryImg.FileName);
                        using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                        {
                            CategoryImg.CopyTo(fileStreams);
                        }
                        obj.CategoryImg = @"\Images\CategoryImage\" + fileName + extension;
                        await unitOfWork.Category.Add(obj);
                        await unitOfWork.Save();
                        ViewBag.msg = "Categories has been Created.";
                    }
                }
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id,string? msg)
        {
            if (msg!=null){
                ViewBag.msg = msg;
            }
            var data = await unitOfWork.Category.GetFirstOrDefault(x => x.CategoryId == id);
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(SuperMarket_Models.Models.Category obj, IFormFile CategoryImg)
        {
            var data = await unitOfWork.Category.GetAll(x => x.CategoryId != obj.CategoryId);
            string wwwRootPath = env.WebRootPath;
            if (data.Count()==0)
            {
                if (CategoryImg != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"Images\CategoryImage");
                    var extension = Path.GetExtension(CategoryImg.FileName);
                    var temp = await unitOfWork.Category.GetFirstOrDefault(x => x.CategoryId == obj.CategoryId);
                    var oldImgPath = Path.Combine(wwwRootPath, temp.CategoryImg.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImgPath))
                    {
                        System.IO.File.Delete(oldImgPath);
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        CategoryImg.CopyTo(fileStreams);
                    }
                    obj.CategoryImg = @"\Images\CategoryImage\" + fileName + extension;
                    obj.UpdateDate = DateTime.Now;
                    unitOfWork.Category.Update(obj);
                    await unitOfWork.Save();
                    return RedirectToAction("UpdateCategory", new { msg = "Categories has been Updated." });
                }
                else
                {
                    var temp = await unitOfWork.Category.GetFirstOrDefault(x => x.CategoryId == obj.CategoryId);
                    obj.CategoryImg = temp.CategoryImg;
                    obj.UpdateDate = DateTime.Now;
                    unitOfWork.Category.Update(obj);
                    await unitOfWork.Save();
                    return RedirectToAction("UpdateCategory", new { msg = "Categories has been Updated." });
                }
                
            }
            else
            {
                foreach (var item in data)
                {
                    if (item.CategoryName.ToLower()==obj.CategoryName.ToLower())
                    {
                        return RedirectToAction("UpdateCategory", new { msg = "Category name has been Used. Try another." });
                    }
                    else
                    {
                        if(CategoryImg != null)
                        {
                            string fileName = Guid.NewGuid().ToString();
                            var uploads = Path.Combine(wwwRootPath, @"Images\CategoryImage");
                            var extension = Path.GetExtension(CategoryImg.FileName);
                            var temp = await unitOfWork.Category.GetFirstOrDefault(x => x.CategoryId == obj.CategoryId);
                            var oldImgPath = Path.Combine(wwwRootPath, temp.CategoryImg.TrimStart('\\'));
                            if (System.IO.File.Exists(oldImgPath))
                            {
                                System.IO.File.Delete(oldImgPath);
                            }

                            using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                            {
                                CategoryImg.CopyTo(fileStreams);
                            }
                            obj.CategoryImg = @"\Images\CategoryImage\" + fileName + extension;
                            obj.UpdateDate = DateTime.Now;
                            unitOfWork.Category.Update(obj);
                            await unitOfWork.Save();
                            return RedirectToAction("UpdateCategory", new { msg = "Categories has been Updated." });
                        }
                        else
                        {
                            var temp = await unitOfWork.Category.GetFirstOrDefault(x => x.CategoryId == obj.CategoryId);
                            obj.CategoryImg = temp.CategoryImg;
                            obj.UpdateDate = DateTime.Now;
                            unitOfWork.Category.Update(obj);
                            await unitOfWork.Save();
                            return RedirectToAction("UpdateCategory", new { msg = "Categories has been Updated." });
                        }
                        

                    }
                }
            
            }
            return null;
        }
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var data = await unitOfWork.Brand_Category.GetAll(x => x.CategoryId == id);

            foreach (var item in data)
            {
                var temp = await unitOfWork.Product.GetFirstOrDefault(x => x.BrandCateId == item.BrandCateId);
                if (temp != null)
                {
                    return Json(new { success = false });
                }
            }
            var data1 = await unitOfWork.Category.GetFirstOrDefault(x => x.CategoryId == id);
            unitOfWork.Category.Remove(data1);
            await unitOfWork.Save();
            return Json(new { success = true });
        }
    }
}

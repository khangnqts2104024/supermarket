using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;

namespace SuperMarket_Client.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment env;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            this.unitOfWork = unitOfWork;
            this.env = env;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProduct()
        {
            var data = await unitOfWork.Product.GetAll(includeProperties: "Brand_Category.Brand,Brand_Category.Category,Stock");
            return Json(new { data = data });
        }
        [HttpGet]
        public async Task<IActionResult> ProductDetail(int id)
        {
            var data = await unitOfWork.Product.GetFirstOrDefault(x => x.ProductId == id, includeProperties: "Brand_Category.Brand,Brand_Category.Category");
            var stock = await unitOfWork.Stock.GetAll(x => x.ProductId == id, includeProperties: "Branch");
            var ImageList = await unitOfWork.ImageProduct.GetAll(x => x.ProductId == id);
            ViewBag.ImageList = ImageList;
            ViewBag.stockList = stock;
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProduct(Product obj, IFormFile file)
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> CreateProduct(string? msg)
        {
            var categoryList = await unitOfWork.Category.GetAll();
            var brandList = await unitOfWork.Brand.GetAll();
            ViewBag.categoryList = new SelectList(categoryList, "CategoryId", "CategoryName");
            ViewBag.brandList = new SelectList(brandList, "BrandId", "BrandName");
            ViewBag.msg = msg;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product obj, IFormFile mainImage, IFormFile[] sideImage, int CategoryId, int BrandId)
        {
            string wwwRootPath = env.WebRootPath;
            var check = await unitOfWork.Brand_Category.GetFirstOrDefault(x => x.CategoryId == CategoryId && x.BrandId == BrandId);
            if (obj != null)
            {
                var data = await unitOfWork.Product.GetAll();
                foreach (var item in data)
                {
                    if (item.ProductName == obj.ProductName)
                    {
                        return RedirectToAction("CreateProduct", "Product", new { msg = "Name of product has been used. Try an other." });
                    }
                }
                if (check == null)
                {
                    var newBrandCate = new Brand_Category()
                    {
                        BrandId = BrandId,
                        CategoryId = CategoryId
                    };
                    await unitOfWork.Brand_Category.Add(newBrandCate);
                    await unitOfWork.Save();
                    var brandcateId = await unitOfWork.Brand_Category.GetFirstOrDefault(x => x.CategoryId == CategoryId && x.BrandId == BrandId);
                    obj.BrandCateId = brandcateId.BrandCateId;
                    TempData["Product"] = JsonConvert.SerializeObject(obj);
                    if (mainImage != null)
                    {
                        string fileName = Guid.NewGuid().ToString();
                        var uploads = Path.Combine(wwwRootPath, @"Images\ProductImage");
                        var extension = Path.GetExtension(mainImage.FileName);
                        var newMainImg = new ImageProduct()
                        {
                            Url = @"\Images\ProductImage\" + fileName + extension,
                            IsMainImage = true
                        };
                        using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                        {
                            mainImage.CopyTo(fileStreams);
                        }
                        TempData["MainImage"] = JsonConvert.SerializeObject(newMainImg);

                    }
                    if (sideImage != null)
                    {
                        var imgList = new List<ImageProduct>();

                        foreach (var item in sideImage)
                        {
                            string fileName = Guid.NewGuid().ToString();
                            var uploads = Path.Combine(wwwRootPath, @"Images\ProductImage");
                            var extension = Path.GetExtension(item.FileName);
                            var newSideImg = new ImageProduct()
                            {
                                Url = @"\Images\ProductImage\" + fileName + extension,
                                IsMainImage = false
                            };
                            using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                            {
                                item.CopyTo(fileStreams);
                            }
                            imgList.Add(newSideImg);
                        }
                        TempData["sideImageList"] = JsonConvert.SerializeObject(imgList);

                    }
                    return RedirectToAction("ProcessCreateProduct", "Product");
                }
                else
                {
                    obj.BrandCateId = check.BrandCateId;
                    await unitOfWork.Product.Add(obj);
                    await unitOfWork.Save();

                    var getId = await unitOfWork.Product.GetFirstOrDefault(x => x.ProductName == obj.ProductName);
                    if (mainImage != null)
                    {
                        string fileName = Guid.NewGuid().ToString();
                        var uploads = Path.Combine(wwwRootPath, @"Images\ProductImage");
                        var extension = Path.GetExtension(mainImage.FileName);

                        using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                        {
                            mainImage.CopyTo(fileStreams);
                        }
                        var MainImage = new ImageProduct()
                        {
                            Url = @"\Images\ProductImage\" + fileName + extension,
                            IsMainImage = true,
                            ProductId = getId.ProductId
                        };
                        TempData["MainImage"] = JsonConvert.SerializeObject(MainImage);
                    }
                    if (sideImage != null)
                    {
                        var uploads = Path.Combine(wwwRootPath, @"Images\ProductImage");
                        var imgList = new List<ImageProduct>();
                        foreach (var item in sideImage)
                        {
                            string fileName = Guid.NewGuid().ToString();
                            var extension = Path.GetExtension(item.FileName);
                            using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                            {
                                item.CopyTo(fileStreams);
                            }
                            var sideImg = new ImageProduct()
                            {
                                Url = @"\Images\ProductImage\" + fileName + extension,
                                IsMainImage = false,
                                ProductId = getId.ProductId
                            };
                            imgList.Add(sideImg);

                        }
                        TempData["sideImageList"] = JsonConvert.SerializeObject(imgList);
                    }
                    return RedirectToAction("ProcessCreateProduct", "Product");

                }



            }
            return View();
        }
        public async Task<IActionResult> ProcessCreateProduct()
        {
            if (TempData["Product"] == null)
            {
                var MainImage = JsonConvert.DeserializeObject<ImageProduct>(TempData["MainImage"].ToString());
                var sideImageList = JsonConvert.DeserializeObject<List<ImageProduct>>(TempData["sideImageList"].ToString());
                await unitOfWork.ImageProduct.Add(MainImage);
                await unitOfWork.ImageProduct.AddRange(sideImageList);
                await unitOfWork.Save();
                return RedirectToAction("CreateProduct", new { msg = "Product is created succesfully" });
            }
            else
            {
                var product = JsonConvert.DeserializeObject<Product>(TempData["Product"].ToString());
                var MainImage = JsonConvert.DeserializeObject<ImageProduct>(TempData["MainImage"].ToString());
                var sideImageList = JsonConvert.DeserializeObject<List<ImageProduct>>(TempData["sideImageList"].ToString());

                await unitOfWork.Product.Add(product);
                await unitOfWork.Save();

                var data = await unitOfWork.Product.GetFirstOrDefault(x => x.ProductName == product.ProductName);
                MainImage.ProductId = data.ProductId;
                foreach (var item in sideImageList)
                {
                    item.ProductId = data.ProductId;
                }
                TempData["fianlMainImg"] = JsonConvert.SerializeObject(MainImage);
                TempData["finalSideImgLsit"] = JsonConvert.SerializeObject(sideImageList);
                return RedirectToAction("LastProcessCreateProduct", "Product");
            }
        }
        public async Task<IActionResult> LastProcessCreateProduct()
        {
            var fianlMainImg = JsonConvert.DeserializeObject<ImageProduct>(TempData["fianlMainImg"].ToString());
            var finalSideImgLsit = JsonConvert.DeserializeObject<List<ImageProduct>>(TempData["finalSideImgLsit"].ToString());
            await unitOfWork.ImageProduct.Add(fianlMainImg);
            await unitOfWork.ImageProduct.AddRange(finalSideImgLsit);
            return RedirectToAction("CreateProduct", "Product", new { msg = "Product is created succesfully" });
        }
        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            var data = await unitOfWork.Product.GetFirstOrDefault(x => x.ProductId == id, includeProperties: "Brand_Category.Brand,Brand_Category.Category");
            var imgList = await unitOfWork.ImageProduct.GetAll(x => x.ProductId == id);
            ViewBag.imgList = imgList;
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Product obj, IFormFile? mainImg, int? mainImgId, IFormFile? sideImg_1, IFormFile? sideImg_2, IFormFile? sideImg_3, IFormFile? sideImg_4, IFormFile? sideImg_5, IFormFile? sideImg_6, int? imgId_1, int? imgId_2, int? imgId_3, int? imgId_4, int? imgId_5, int? imgId_6, string? mainImageUrl, string? sideImageUrl_1, string? sideImageUrl_2, string? sideImageUrl_3, string? sideImageUrl_4, string? sideImageUrl_5, string? sideImageUrl_6, IFormFile? AddNewSideImg_1, IFormFile? AddNewSideImg_2, IFormFile? AddNewSideImg_3, IFormFile? AddNewSideImg_4, IFormFile? AddNewSideImg_5, IFormFile? AddNewSideImg_6)
        {
            var data = await unitOfWork.Product.GetAll(x => x.ProductId != obj.ProductId);
            string wwwRootPath = env.WebRootPath;
            foreach (var item in data)
            {
                if (item.ProductName == obj.ProductName)
                {
                    ViewBag.msg = "Product name has been used. Please try another";
                    return RedirectToAction("UpdateProduct", new { id = obj.ProductId });
                }
            }
            unitOfWork.Product.Update(obj);
            if (mainImg != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Images\ProductImage");
                var extension = Path.GetExtension(mainImg.FileName);
                var oldImgPath = Path.Combine(wwwRootPath, mainImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImgPath))
                {
                    System.IO.File.Delete(oldImgPath);
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    mainImg.CopyTo(fileStreams);
                }
                var newMainImg = new ImageProduct()
                {
                    ImageId = (int)mainImgId,
                    Url = @"\Images\ProductImage\" + fileName + extension,
                    IsMainImage = true,
                    ProductId = obj.ProductId
                };
                unitOfWork.ImageProduct.Update(newMainImg);
            }
            if (sideImg_1 != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Images\ProductImage");
                var extension = Path.GetExtension(sideImg_1.FileName);
                var oldImgPath = Path.Combine(wwwRootPath, sideImageUrl_1.TrimStart('\\'));
                if (System.IO.File.Exists(oldImgPath))
                {
                    System.IO.File.Delete(oldImgPath);
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    sideImg_1.CopyTo(fileStreams);
                }
                var newSideImg = new ImageProduct()
                {
                    ImageId = (int)imgId_1,
                    Url = @"\Images\ProductImage\" + fileName + extension,
                    IsMainImage = false,
                    ProductId = obj.ProductId
                };
                unitOfWork.ImageProduct.Update(newSideImg);
            }
            if (sideImg_2 != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Images\ProductImage");
                var extension = Path.GetExtension(sideImg_2.FileName);
                var oldImgPath = Path.Combine(wwwRootPath, sideImageUrl_2.TrimStart('\\'));
                if (System.IO.File.Exists(oldImgPath))
                {
                    System.IO.File.Delete(oldImgPath);
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    sideImg_2.CopyTo(fileStreams);
                }
                var newSideImg = new ImageProduct()
                {
                    ImageId = (int)imgId_2,
                    Url = @"\Images\ProductImage\" + fileName + extension,
                    IsMainImage = false,
                    ProductId = obj.ProductId
                };
                unitOfWork.ImageProduct.Update(newSideImg);
            }
            if (sideImg_3 != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Images\ProductImage");
                var extension = Path.GetExtension(sideImg_3.FileName);
                var oldImgPath = Path.Combine(wwwRootPath, sideImageUrl_3.TrimStart('\\'));
                if (System.IO.File.Exists(oldImgPath))
                {
                    System.IO.File.Delete(oldImgPath);
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    sideImg_3.CopyTo(fileStreams);
                }
                var newSideImg = new ImageProduct()
                {
                    ImageId = (int)imgId_3,
                    Url = @"\Images\ProductImage\" + fileName + extension,
                    IsMainImage = false,
                    ProductId = obj.ProductId
                };
                unitOfWork.ImageProduct.Update(newSideImg);
            }
            if (sideImg_4 != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Images\ProductImage");
                var extension = Path.GetExtension(sideImg_4.FileName);
                var oldImgPath = Path.Combine(wwwRootPath, sideImageUrl_4.TrimStart('\\'));
                if (System.IO.File.Exists(oldImgPath))
                {
                    System.IO.File.Delete(oldImgPath);
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    sideImg_4.CopyTo(fileStreams);
                }
                var newSideImg = new ImageProduct()
                {
                    ImageId = (int)imgId_4,
                    Url = @"\Images\ProductImage\" + fileName + extension,
                    IsMainImage = false,
                    ProductId = obj.ProductId
                };
                unitOfWork.ImageProduct.Update(newSideImg);
            }
            if (sideImg_5 != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Images\ProductImage");
                var extension = Path.GetExtension(sideImg_5.FileName);
                var oldImgPath = Path.Combine(wwwRootPath, sideImageUrl_5.TrimStart('\\'));
                if (System.IO.File.Exists(oldImgPath))
                {
                    System.IO.File.Delete(oldImgPath);
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    sideImg_5.CopyTo(fileStreams);
                }
                var newSideImg = new ImageProduct()
                {
                    ImageId = (int)imgId_5,
                    Url = @"\Images\ProductImage\" + fileName + extension,
                    IsMainImage = false,
                    ProductId = obj.ProductId
                };
                unitOfWork.ImageProduct.Update(newSideImg);
            }
            if (sideImg_6 != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Images\ProductImage");
                var extension = Path.GetExtension(sideImg_6.FileName);
                var oldImgPath = Path.Combine(wwwRootPath, sideImageUrl_6.TrimStart('\\'));
                if (System.IO.File.Exists(oldImgPath))
                {
                    System.IO.File.Delete(oldImgPath);
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    sideImg_6.CopyTo(fileStreams);
                }
                var newSideImg = new ImageProduct()
                {
                    ImageId = (int)imgId_6,
                    Url = @"\Images\ProductImage\" + fileName + extension,
                    IsMainImage = false,
                    ProductId = obj.ProductId
                };
                unitOfWork.ImageProduct.Update(newSideImg);
            }
            if (AddNewSideImg_1 != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Images\ProductImage");
                var extension = Path.GetExtension(AddNewSideImg_1.FileName);
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    AddNewSideImg_1.CopyTo(fileStreams);
                }
                var newSideImg = new ImageProduct()
                {
                    Url = @"\Images\ProductImage\" + fileName + extension,
                    IsMainImage = false,
                    ProductId = obj.ProductId
                };
                await unitOfWork.ImageProduct.Add(newSideImg);
            }
            if (AddNewSideImg_2 != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Images\ProductImage");
                var extension = Path.GetExtension(AddNewSideImg_2.FileName);
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    AddNewSideImg_2.CopyTo(fileStreams);
                }
                var newSideImg = new ImageProduct()
                {
                    Url = @"\Images\ProductImage\" + fileName + extension,
                    IsMainImage = false,
                    ProductId = obj.ProductId
                };
                await unitOfWork.ImageProduct.Add(newSideImg);
            }
            if (AddNewSideImg_3 != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Images\ProductImage");
                var extension = Path.GetExtension(AddNewSideImg_3.FileName);
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    AddNewSideImg_3.CopyTo(fileStreams);
                }
                var newSideImg = new ImageProduct()
                {
                    Url = @"\Images\ProductImage\" + fileName + extension,
                    IsMainImage = false,
                    ProductId = obj.ProductId
                };
                await unitOfWork.ImageProduct.Add(newSideImg);
            }
            if (AddNewSideImg_4 != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Images\ProductImage");
                var extension = Path.GetExtension(AddNewSideImg_4.FileName);
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    AddNewSideImg_4.CopyTo(fileStreams);
                }
                var newSideImg = new ImageProduct()
                {
                    Url = @"\Images\ProductImage\" + fileName + extension,
                    IsMainImage = false,
                    ProductId = obj.ProductId
                };
                await unitOfWork.ImageProduct.Add(newSideImg);
            }
            if (AddNewSideImg_5 != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Images\ProductImage");
                var extension = Path.GetExtension(AddNewSideImg_5.FileName);
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    AddNewSideImg_5.CopyTo(fileStreams);
                }
                var newSideImg = new ImageProduct()
                {
                    Url = @"\Images\ProductImage\" + fileName + extension,
                    IsMainImage = false,
                    ProductId = obj.ProductId
                };
                await unitOfWork.ImageProduct.Add(newSideImg);
            }
            if (AddNewSideImg_6 != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Images\ProductImage");
                var extension = Path.GetExtension(AddNewSideImg_6.FileName);
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    AddNewSideImg_6.CopyTo(fileStreams);
                }
                var newSideImg = new ImageProduct()
                {
                    Url = @"\Images\ProductImage\" + fileName + extension,
                    IsMainImage = false,
                    ProductId = obj.ProductId
                };
                await unitOfWork.ImageProduct.Add(newSideImg);
            }
            await unitOfWork.Save();

            return RedirectToAction("UpdateProduct", new { id = obj.ProductId });
        }

        public async Task<IActionResult> DeleteProduct(int id)
        {
            var check=await unitOfWork.Stock.GetFirstOrDefault(x=>x.ProductId==id);
            if (check != null)
            {
                return Json(new { success = false });
            }
            else
            {
                var data = await unitOfWork.Product.GetFirstOrDefault(x => x.ProductId == id);
                unitOfWork.Product.Remove(data);
                await unitOfWork.Save();
                return Json(new { success = true });
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;

namespace SuperMarket_Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class Brand_CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public Brand_CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> Brand_CategoryManage()
        {
            try
            {
                var data = await unitOfWork.Brand_Category.GetAll(includeProperties: "Brand,Category");
                var categoryList = await unitOfWork.Category.GetAll();
                var product = await unitOfWork.Product.GetAll();
                ViewBag.product = product;
                ViewBag.CategoryList = categoryList;
                return View(data);
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Error", new { area = "Customer" });

            }

        }

        //[HttpGet]
        //public async Task<IActionResult> Brand_CategoryManage()
        //{
        //    //var brand_cate = await unitOfWork.Brand_Category.GetAll(includeProperties: "Brand,Category.Product");
        //    var brand_cate = await unitOfWork.Product.GetAll(includeProperties: "Brand_Category.Brand,Brand_Category.Category");
        //    var results = from b in brand_cate
        //                  group b by b.Brand_Category.Category.CategoryId into g
        //                  select new
        //                  {
        //                      Product = g.Select(g => g.ProductName).ToList(),
        //                      Category = g.Key,
        //                      Brand = g.Select(g => g.Brand_Category.Brand.BrandName).ToList()/**/
        //                  };
        //    var categoryList = await unitOfWork.Category.GetAll();
        //    var product = await unitOfWork.Product.GetAll();
        //    ViewBag.product = product;
        //    ViewBag.CategoryList = categoryList;
        //    return Json(new
        //    {
        //        brand_cate = brand_cate,
        //    });
        //}
    }
}

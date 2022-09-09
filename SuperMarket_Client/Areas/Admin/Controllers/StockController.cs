using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;
using SuperMarket_Models.ViewModels;

namespace SuperMarket_Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class StockController : Controller
    {

        private readonly IUnitOfWork unitOfWork;
        public StockController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {

            var model = await unitOfWork.Product.GetAll(includeProperties: "Brand_Category.Brand,Brand_Category.Category,Stock,Stock.Branch");


            return View(model);
        }

        //[HttpGet]
        //public async Task<IActionResult> UpdateStock(int stockid)
        //{


        //    return View();
        //}


        [HttpGet()]
        public async Task<IActionResult> AddStock(int id)
        {
          
         
        //get stock
            var stock = await unitOfWork.Stock.GetFirstOrDefault(s => s.StockId.Equals(id),includeProperties:"Product,Branch");

            if (stock != null)
            {
               ViewBag.productName= stock.Product.ProductName;
               ViewBag.branchName =stock.Branch.BranchName;

                return View(stock);
            }
            else
            {
                return View("404");
            }
        
           
        }
        [HttpPost]
        public async Task<IActionResult> AddStock(Stock stock,int number)
        {

            var model= await unitOfWork.Stock.GetFirstOrDefault(s => s.StockId.Equals(stock.StockId));
            if (model != null)
            {
                 unitOfWork.Stock.IncrementStock(model,number);
                await unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else 
            {
                return View("404");
            }

          
        }


        [HttpGet()]
        public async Task<IActionResult> UpdateStock(int id)
        {


            //get stock
            var stock = await unitOfWork.Stock.GetFirstOrDefault(s => s.StockId.Equals(id), includeProperties: "Product,Branch");

            if (stock != null)
            {
               

                return View(stock);
            }
            else
            {
                return View("404");
            }


        }
        [HttpPost]
        public async Task<IActionResult> UpdateStock(Stock stock, int number)
        {

            var model = await unitOfWork.Stock.GetFirstOrDefault(s => s.StockId.Equals(stock.StockId));
            if (model != null)
            {
                unitOfWork.Stock.UpdateStock(model, number);
                await unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View("404");
            }


        }


    }
}

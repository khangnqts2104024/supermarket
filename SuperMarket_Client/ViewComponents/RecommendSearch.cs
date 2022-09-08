using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;

namespace SuperMarket_Client.ViewComponents
{
    [ViewComponent(Name = "RecommendSearch")]
    [BranchActionFilter]
    public class RecommendSearchViewComponent : ViewComponent
    {
        private readonly IUnitOfWork unitOfWork;

        public RecommendSearchViewComponent(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                if (HttpContext.Request.Cookies["branchId"] != null)
                {
                    var branchId = int.TryParse(HttpContext.Request.Cookies["branchId"], out int branchid);
                    var stockList = await unitOfWork.Stock.GetAll(x => x.BranchId == branchid && x.Count > 0, includeProperties: "Product.Brand_Category.Category,Product.ImageProduct");
                    ViewBag.CategoryList = await unitOfWork.Category.GetAll();
                    var rnd = new Random();
                    var _result = stockList.OrderBy(x => rnd.Next()).Take(3);
                    return View("RecommendSearch", _result);

                }
                var result = "";
                return View("RecommendSearch", result);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

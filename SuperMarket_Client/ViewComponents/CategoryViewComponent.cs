using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;

namespace SuperMarket_Client.ViewComponents
{
    [ViewComponent(Name = "CategoryList")]

    public class CategoryViewComponent:ViewComponent
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryViewComponent(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categoryList = await unitOfWork.Category.GetAll();

            return View("CategoryList", categoryList);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperMarket_DataAccess.Repository.IRepository;

namespace SuperMarket_Client.ViewComponents
{
    [ViewComponent(Name = "BranchList")]

    public class BranchViewComponent:ViewComponent
    {
        private readonly IUnitOfWork unitOfWork;
        public BranchViewComponent(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var List = await unitOfWork.Branch.GetAll();
            return View("BranchList",List);
        }

        

        
    }
}

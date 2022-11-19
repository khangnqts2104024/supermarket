using Microsoft.AspNetCore.Mvc;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;

namespace SuperMarket_Client.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class BranchController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment env;
        public BranchController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            this.unitOfWork = unitOfWork;
            this.env = env;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetAllBranch()
        {
            var data = await unitOfWork.Branch.GetAll();
            return Json(new { data = data });
        }
        [HttpGet]
        public async Task<IActionResult> CreateBranch(string? msg)
        {
            if (msg != null)
            {
                ViewBag.msg = msg;
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateBranch(Branch obj,IFormFile BranchImg)
        {
            string wwwRootPath = env.WebRootPath;

            if (obj != null && BranchImg !=null )
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Images\BranchImage");
                var extension = Path.GetExtension(BranchImg.FileName);
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    BranchImg.CopyTo(fileStreams);
                }
                obj.BranchImg = @"\Images\BranchImage\" + fileName + extension;
                await unitOfWork.Branch.Add(obj);
                await unitOfWork.Save();
                var msg = "Add successfully";
                return RedirectToAction("CreateBranch", new { msg = msg });

            }
            else
            {
                var msg = "Some thing went wrong.";
                return RedirectToAction("CreateBranch", new { msg = msg });

            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            string wwwRootPath = env.WebRootPath;

            var data = await unitOfWork.Stock.GetFirstOrDefault(x => x.BranchId == id);
            if(data == null)
            {
                var temp=await unitOfWork.Branch.GetFirstOrDefault(x => x.BranchId == id);
                var oldImgPath = Path.Combine(wwwRootPath, temp.BranchImg.TrimStart('\\'));
                if (System.IO.File.Exists(oldImgPath))
                {
                    System.IO.File.Delete(oldImgPath);
                }
                unitOfWork.Branch.Remove(temp);
                await unitOfWork.Save();
                return Json(new { success = true, msg = "Branch has been deleted." });

            }
            else
            {
                return Json(new { success = false, msg = "This Branch's Stocks are still available, can not delete unless delete all Stocks." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> UpdateBranch(int id,string msg)
        {
            var data = await unitOfWork.Branch.GetFirstOrDefault(x => x.BranchId == id);
            if (msg != null)
            {
                ViewBag.msg = msg;
            }
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBranch(Branch obj,IFormFile BranchImg)
        {
            string wwwRootPath = env.WebRootPath;
            if (BranchImg != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Images\BranchImage");
                var extension = Path.GetExtension(BranchImg.FileName);
                var temp = await unitOfWork.Branch.GetFirstOrDefault(x => x.BranchId == obj.BranchId);
                var oldImgPath = Path.Combine(wwwRootPath, temp.BranchImg.TrimStart('\\'));
                if (System.IO.File.Exists(oldImgPath))
                {
                    System.IO.File.Delete(oldImgPath);
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    BranchImg.CopyTo(fileStreams);
                }
                obj.BranchImg = @"\Images\BranchImage\" + fileName + extension;
                unitOfWork.Branch.Update(obj);
                await unitOfWork.Save();
                return RedirectToAction("UpdateBranch", new { id = obj.BranchId, msg = "Branch has been Updated." });
            }
            else
            {
                var temp = await unitOfWork.Branch.GetFirstOrDefault(x => x.BranchId == obj.BranchId);
                obj.BranchImg = temp.BranchImg;
                unitOfWork.Branch.Update(obj);
                await unitOfWork.Save();
                return RedirectToAction("UpdateBranch", new {id=obj.BranchId, msg = "Branch has been Updated." });
            }
            return null;
        }

    }
}

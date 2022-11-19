using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;
using SuperMarket_Utility;

namespace SuperMarket_Client.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class StoreLocatorController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
   
        public StoreLocatorController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
       

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(string fullname, string email, string phone, string message)
        {
            string html = "<h3>" + fullname + "</h3>" +
                "<p>" + email + "</p>" +
                "<p>Phone: " + phone + "</p>" +
                "<p> Message: " + message + "</p>";

            string subject = "Customer have a question!";
            EmailSender emailSender = new EmailSender();
            await emailSender.UserSendEmailAsync(subject, html);
            TempData["MsgSuccess"] = "Your Mail Has Been Sent!";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> FindShortestBranch(string latituteUser, string longitudeUser)
        {
            var branch = await unitOfWork.Branch.GetAll();
            if(branch != null)
            {
                    if (!string.IsNullOrEmpty(latituteUser) && !string.IsNullOrEmpty(longitudeUser))
                    {
                        Location currentUserLocation = new Location()
                        {
                            Latitude = double.Parse(latituteUser),
                            Longitude = double.Parse(longitudeUser)

                        };
                        List<Location> storeLocation = new List<Location>();
                        List<double> distances = new List<double>();
                        foreach (var item in branch)
                        {
                            Location brachLocation = new Location()
                            {
                                Latitude = double.Parse(item.Latitude),
                                Longitude = double.Parse(item.Longtitude),
                                BranchId = item.BranchId,
                                BranchName = item.BranchName
                            };
                            brachLocation.DistanceToUser = CalculateDistance(brachLocation, currentUserLocation);
                            storeLocation.Add(brachLocation);
                        }

                        var shortestDistance = storeLocation.Min(x => x.DistanceToUser);
                        var selectbranch = storeLocation.FirstOrDefault(x => x.DistanceToUser == shortestDistance);
                        if(selectbranch != null)
                        {
                            var shortestBranch = branch.FirstOrDefault(x => x.BranchId == selectbranch.BranchId);
                            if(shortestBranch != null)
                            {
                                return Json(new
                                {
                                    data = shortestBranch
                                });
                            }
                            else
                            {
                                return Json(new
                                {
                                    data = ""
                                });
                            }
                        }
                    }
                
            }
            

            return null;
        }

        public double CalculateDistance(Location location1, Location currentUserLocation)
        {
            try
            {
                var d1 = location1.Latitude * (Math.PI / 180.0);
                var num1 = location1.Longitude * (Math.PI / 180.0);
                var d2 = currentUserLocation.Latitude * (Math.PI / 180.0);
                var num2 = currentUserLocation.Longitude * (Math.PI / 180.0) - num1;
                var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                         Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
                return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
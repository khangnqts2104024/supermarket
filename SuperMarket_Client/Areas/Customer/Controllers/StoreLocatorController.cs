using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;

namespace SuperMarket_Client.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class StoreLocatorController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly HttpClient _httpClient;
        //create constructor and call HttpClient
        public StoreLocatorController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            _httpClient = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(5)
            };
        }
        private async Task<string> GetIPAddress()
        {
            var ipAddress = await _httpClient.GetAsync($"http://ipinfo.io/ip");
            if (ipAddress.IsSuccessStatusCode)
            {
                var json = await ipAddress.Content.ReadAsStringAsync();
                return json.ToString();
            }
            return "";
        }

        public async Task<string> GetGeoInfo()
        {
            //I have already created this function under GeoInfoProvider class.
            var ipAddress = await GetIPAddress();
            // When geting ipaddress, call this function and pass ipaddress as given below
            var response = await _httpClient.GetAsync($"http://api.ipstack.com/" + ipAddress + "?access_key=314a29c1b728e0c80705ef749c0e4059");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return json;
            }
            return null;
        }

        public async Task<IActionResult> Index()
        {
            Branch shortestbranch = await FindShortestBranch();
            return View(shortestbranch);
        }


        public async Task<Branch> FindShortestBranch()
        {
            var branch = await unitOfWork.Branch.GetAll();
            var result = await GetGeoInfo();
            string latituteUser = JObject.Parse(result)["latitude"].ToString();
            string longitudeUser = JObject.Parse(result)["longitude"].ToString();
            Location currentUserLocation = new Location()
            {
                Latitude = Double.Parse(latituteUser),
                Longitude = Double.Parse(longitudeUser)
            };
            List<Location> storeLocation = new List<Location>();
            List<double> distances = new List<double>(); 
            foreach (var item in branch)
            {
                Location brachLocation = new Location()
                {
                    Latitude = Double.Parse(item.Latitude),
                    Longitude = Double.Parse(item.Longtitude),
                    BranchId = item.BranchId,
                    BranchName = item.BranchName
                };
                brachLocation.DistanceToUser = CalculateDistance(brachLocation, currentUserLocation);
                storeLocation.Add(brachLocation);
            }

            var shortestDistance = storeLocation.Max(x => x.DistanceToUser);
            var selectbranch = storeLocation.FirstOrDefault(x => x.DistanceToUser == shortestDistance);
            var shortestBranch = branch.FirstOrDefault(x => x.BranchId == selectbranch.BranchId);
            return shortestBranch;

        }

        public double CalculateDistance(Location location1, Location currentUserLocation)
        {
            var d1 = location1.Latitude * (Math.PI / 180.0);
            var num1 = location1.Longitude * (Math.PI / 180.0);
            var d2 = currentUserLocation.Latitude * (Math.PI / 180.0);
            var num2 = currentUserLocation.Longitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }
    }
}
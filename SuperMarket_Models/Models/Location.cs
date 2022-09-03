using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_Models.Models
{
    public class Location
    {
            public string BranchName { get; set; }
            public int BranchId { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double DistanceToUser { get; set; }
    }
}

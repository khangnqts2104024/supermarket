using SuperMarket_DataAccess.Data;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_DataAccess.Services.Generic_Imp;
using SuperMarket_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_DataAccess.Services
{
    public class ImageProductService: Repository<ImageProduct>, IImageProduct
    {
        private readonly ApplicationDbContext _db;
        public ImageProductService(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ImageProduct obj)
        {
            throw new NotImplementedException();
        }
    }
}

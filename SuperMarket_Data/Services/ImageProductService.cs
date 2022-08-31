using SuperMarket_DataAccess.Data;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_DataAccess.Services.Generic_Imp;
using SuperMarket_Models.Models;


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
            var objFromDb = _db.ImageProducts.FirstOrDefault(x => x.ImageId == obj.ImageId);
            if (objFromDb != null)
            {

                objFromDb.Url = obj.Url;
                objFromDb.ProductId = obj.ProductId;
                
                _db.ImageProducts.Update(objFromDb);
            }
        }
    }
}

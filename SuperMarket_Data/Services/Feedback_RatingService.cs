using SuperMarket_DataAccess.Data;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_DataAccess.Services.Generic_Imp;
using SuperMarket_Models.Models;


namespace SuperMarket_DataAccess.Services
{
    public class Feedback_RatingService : Repository<Feedback_Rating>, IFeedback_Rating
    {
        private readonly ApplicationDbContext _db;
        public Feedback_RatingService(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(Feedback_Rating obj)
        {

        }
    }
}

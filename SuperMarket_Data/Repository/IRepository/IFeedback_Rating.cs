using SuperMarket_DataAccess.Repository.IRepository.GenericInterface;
using SuperMarket_Models.Models;


namespace SuperMarket_DataAccess.Repository.IRepository
{
    public interface IFeedback_Rating : IRepository<Feedback_Rating>
    {
        void Update(Feedback_Rating obj);
    }
}

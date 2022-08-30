using SuperMarket_DataAccess.Repository.IRepository.GenericInterface;
using SuperMarket_Models.Models;


namespace SuperMarket_DataAccess.Repository.IRepository
{
    public interface IBrand_Category : IRepository<Brand_Category>
    {
        void Update(Brand_Category obj);

    }
}

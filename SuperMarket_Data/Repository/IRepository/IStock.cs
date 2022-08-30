using SuperMarket_DataAccess.Repository.IRepository.GenericInterface;
using SuperMarket_Models.Models;


namespace SuperMarket_DataAccess.Repository.IRepository
{
    public interface IStock: IRepository<Stock>
    {

        void IncrementStock(Stock obj,int count);
        void DecrementStock(Stock obj,int count);
    }
}

using SuperMarket_DataAccess.Repository.IRepository.GenericInterface;
using SuperMarket_Models.Models;


namespace SuperMarket_DataAccess.Repository.IRepository
{
    public interface IShoppingCart :IRepository<ShoppingCart>
    {
        int IncrementCount(ShoppingCart shoppingCart, int count);
        int DecrementCount(ShoppingCart shoppingCart, int count);

    }
}

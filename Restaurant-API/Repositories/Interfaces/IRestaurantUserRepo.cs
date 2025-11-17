using Restaurant_API.Data.Entities;

namespace Restaurant_API.Repositories.Interfaces
{
    public interface IRestaurantUserRepo
    {
        Task<Restaurant?> GetRestaurantByUserIdAsync(Guid userId);
    }
}

using Restaurant_API.Data.Entities;

namespace Restaurant_API.Repositories.Interfaces
{
    public interface IRestaurantRepo
    {
        Task<Restaurant?> GetByIdAsync(Guid id);
    }
}

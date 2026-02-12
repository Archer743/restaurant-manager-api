using Restaurant_API.Data.Entities;
using Restaurant_API.Data.Entities.Enums;

namespace Restaurant_API.Repositories.Interfaces
{
    public interface IReservationRepo
    {
        Task AddAsync(Reservation reservation);

		Task UpdateAsync(Reservation reservation);

		Task RemoveAsync(Reservation reservation);

		Task<int> RemoveReservationsCreatedBeforeAsync(DateTime cutoffUtc);

		Task<Reservation?> GetByIdAsync(Guid id);

		Task<Reservation?> GetByIdWithDetailsAsync(Guid id);

		Task<Reservation?> GetByTagAsync(Guid restaurantId, string tag);

		IQueryable<Reservation> GetPendingReservationsBaseQuery(Guid restaurantId);

		Task<List<Reservation>> GetConfirmedWithinUtcRangeAsync(Guid restaurantId, DateTime rangeStartUtc, DateTime rangeEndUtc);

        Task<HashSet<Guid>> GetReservedTableIdsInPeriodAsync(Guid restaurantId, DateTime periodStartUtc, DateTime periodEndUtc);

        /// <returns>A dictionary where the key is the table's ID and the value is the ID of the reservation currently occupying that table.</returns>
        Task<Dictionary<Guid, Guid>> GetReservedTablesInPeriodAsync(Guid restaurantId, DateTime periodStartUtc, DateTime periodEndUtc);
	}
}

using Restaurant_API.Data.Entities;
using Restaurant_API.Data.Models.Other;
using Restaurant_API.Data.Models.Reservation;

namespace Restaurant_API.Services.Interfaces
{
    public interface IReservationService
    {
        Task CreateClientReservationAsync(ClientCreateReservationModel createModel);

        Task CreateConfirmedReservationAsync(HttpContext ctx, ReservationCreateModel createModel);

        Task<ReservationViewModel> GetByIdAsync(HttpContext ctx, Guid reservationId);

        Task<FloorPlanReservationPreviewModel> GetFloorPlanReservationPreviewAsync(HttpContext ctx, Guid reservationId);

        Task<Dictionary<DateOnly, List<ReservationCalendarPreviewModel>>> GetConfirmedReservationsInPeriodAsync(HttpContext ctx, DateOnly start, DateOnly end);

        Task<PagedResult<PendingReservationPreviewModel>> GetPendingReservationsAsync(HttpContext ctx, PendingReservationsQueryParams queryParams);

        Task CloseReservationAsync(HttpContext ctx, Guid reservationId);

        Task ReopenReservationAsync(HttpContext ctx, Guid reservationId);

        Task<ReservationViewModel> UpdateAsync(HttpContext ctx, Guid reservationId, ReservationUpdateModel updateModel);

        Task RemoveAsync(HttpContext ctx, Guid reservationId);

		/// <summary>
		/// Cleans up all reservations that have passed the configured retention period.
		/// </summary>
		Task CleanupExpiredReservationsAsync();

        Task<Reservation> GetReservationIfOwnedAsync(HttpContext ctx, Guid reservationId, bool includeDetails = false);
    }
}

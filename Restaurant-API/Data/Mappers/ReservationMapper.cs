using Restaurant_API.Data.Entities;
using Restaurant_API.Data.Models.Reservation;

namespace Restaurant_API.Data.Mappers
{
    public static class ReservationMapper
    {
        public static Reservation ToEntity(this ClientCreateReservationModel createModel)
        {
            return new Reservation
            {
                // Tag is set in ReservationService
                RestaurantId = createModel.RestaurantId,
                Name = createModel.Name,
                StartTimeUtc = createModel.StartTimeUtc,
                PeopleCount = createModel.PeopleCount,
                Email = createModel.Email,
                Phone = createModel.Phone,
                Comment = createModel.Comment,
            };
        }

        public static Reservation ToEntity(this ReservationCreateModel createModel)
        {
            return new Reservation
            {
                // Tag is set in ReservationService
                // RestaurantId is set in ReservationService based on the current user
                Name = createModel.Name,
                StartTimeUtc = createModel.StartTimeUtc,
                PeopleCount = createModel.PeopleCount,
                Email = createModel.Email,
                Phone = createModel.Phone,
                Comment = createModel.Comment,
            };
        }

        public static FloorPlanReservationPreviewModel ToFloorPlanPreview(this Reservation reservation, TimeZoneInfo restaurantTimeZone)
        {
            return new FloorPlanReservationPreviewModel
            {
                Id = reservation.Id,
                Tag = reservation.Tag,
                Name = reservation.Name,
                // Converts StartTimeUtc to local time based on the restaurant time zone and extracts the time
                Time = TimeOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(reservation.StartTimeUtc, restaurantTimeZone)),
                PeopleCount = reservation.PeopleCount,
                // TableCount is set in ReservationService
            };
        }

        public static ReservationCalendarPreviewModel ToCalendarPreview(this Reservation reservation, TimeZoneInfo restaurantTimeZone)
        {
            return new ReservationCalendarPreviewModel
            {
                Id = reservation.Id,
                Tag = reservation.Tag,
                Name = reservation.Name,
                // Converts StartTimeUtc to local time based on the restaurant time zone and extracts the time
                Time = TimeOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(reservation.StartTimeUtc, restaurantTimeZone)),
                PeopleCount = reservation.PeopleCount,
                IsClosed = reservation.IsClosed,
            };
        }

        public static PendingReservationPreviewModel ToPendingPreview(this Reservation reservation)
        {
            return new PendingReservationPreviewModel
            {
                Id = reservation.Id,
                Tag = reservation.Tag,
                Name = reservation.Name,
                StartTimeUtc = reservation.StartTimeUtc,
                CreatedAtUtc = reservation.CreatedAtUtc,
                PeopleCount = reservation.PeopleCount,
            };
        }

        public static ReservationViewModel ToView(this Reservation reservation)
        {
            return new ReservationViewModel
            {
                Id = reservation.Id,
                Tag = reservation.Tag,
                Name = reservation.Name,
                StartTimeUtc = reservation.StartTimeUtc,
                PeopleCount = reservation.PeopleCount,
                Email = reservation.Email,
                Phone = reservation.Phone,
                Comment = reservation.Comment,
                IsCreatedByStaff = reservation.IsCreatedByStaff,
                IsClosed = reservation.IsClosed,
            };
        }
    }
}

using Restaurant_API.Data.Entities;
using Restaurant_API.Data.Models.Shift;

namespace Restaurant_API.Data.Mappers
{
    public static class ShiftMapper
    {
        public static ShiftViewModel ToView(this Shift shift)
        {
            return new ShiftViewModel
            {
                Id = shift.Id,
                Name = shift.Name,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
            };
        }

        public static ReservationShiftViewModel ToReservationView(this Shift shift)
        {
            return new ReservationShiftViewModel
            {
                Name = shift.Name,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                // Tables are set in TableService
            };
        }
    }
}

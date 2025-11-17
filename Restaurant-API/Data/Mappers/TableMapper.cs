using Restaurant_API.Data.Entities;
using Restaurant_API.Data.Models.Table;

namespace Restaurant_API.Data.Mappers
{
    public static class TableMapper
    {
        public static TableAvailabilityModel ToAvailabilityView(this Table table)
        {
            return new TableAvailabilityModel
            {
                Id = table.Id,
                TableNumber = table.TableNumber,
                Capacity = table.Capacity,
                // Status is set in TableService
            };
        }

        public static FloorPlanTableAvailabilityModel ToFloorPlanAvailabilityView(this Table table)
        {
            return new FloorPlanTableAvailabilityModel
            {
                Id = table.Id,
                TableNumber = table.TableNumber,
                Capacity = table.Capacity,
                // Status and ReservationId are set in TableService
            };
        }
    }
}

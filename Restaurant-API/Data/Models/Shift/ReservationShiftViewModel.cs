using Restaurant_API.Data.Models.Table;

namespace Restaurant_API.Data.Models.Shift
{
    public class ReservationShiftViewModel
    {
        public string Name { get; set; } = string.Empty;

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public List<TableAvailabilityModel> Tables { get; set; } = [];
    }
}

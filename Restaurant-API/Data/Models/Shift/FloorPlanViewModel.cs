using Restaurant_API.Data.Models.Table;

namespace Restaurant_API.Data.Models.Shift
{
    public class FloorPlanViewModel
    {
        public List<FloorPlanTableAvailabilityModel> Tables { get; set; } = [];

        public List<ShiftViewModel>? Shifts { get; set; }

        public int? SelectedShiftIndex { get; set; }
    }
}

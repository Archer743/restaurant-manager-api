using ShiftEntity = Restaurant_API.Data.Entities.Shift;

namespace Restaurant_API.Data.Models.Shift
{
    public class ShiftContext
    {
        public ShiftEntity Shift { get; set; }

        public HashSet<Guid> ReservedTableIds { get; set; }
    }
}

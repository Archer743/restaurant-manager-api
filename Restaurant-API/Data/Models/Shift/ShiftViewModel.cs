namespace Restaurant_API.Data.Models.Shift
{
    public class ShiftViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }
    }
}

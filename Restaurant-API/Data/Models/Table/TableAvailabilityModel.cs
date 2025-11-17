namespace Restaurant_API.Data.Models.Table
{
    public enum TableStatus
    {
        Available,
        Reserved,
        ReservedByCurrent
    }

    public class TableAvailabilityModel
    {
        public Guid Id { get; set; }

        public int TableNumber { get; set; }

        public int Capacity { get; set; }

        public TableStatus Status { get; set; }
    }
}

namespace Restaurant_API.Data.Models.Table
{
    public class ReservationTablesUpdate
    {
        public List<Guid>? TablesToAdd { get; set; }

        public List<Guid>? TablesToRemove { get; set; }
    }
}

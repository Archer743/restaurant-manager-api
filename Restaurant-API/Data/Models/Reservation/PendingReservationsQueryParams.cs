using Restaurant_API.Data.Models.Reservation.Enums;
using System.ComponentModel.DataAnnotations;

namespace Restaurant_API.Data.Models.Reservation
{
    public class PendingReservationsQueryParams
    {
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 100)]
        public int Limit { get; set; } = 20;

        public SortOrder SortOrder { get; set; } = SortOrder.Desc;
    }
}

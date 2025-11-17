namespace Restaurant_API.Data.Models.Reservation
{
    public class ClientCreateReservationModel : ReservationCreateModel
    {
        public Guid RestaurantId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant_API.Data.Entities
{
    public class RestaurantUser
    {
        // Has composite primary key (RestaurantId + UserId)

        [ForeignKey(nameof(Restaurant))]
        public Guid RestaurantId { get; set; }

        public Restaurant Restaurant { get; set; }


        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public User User { get; set; }


        public DateOnly MemberSince { get; set; }
    }
}

namespace Restaurant_API.Data.Models.Auth
{
    public class UserAuthModel
    {
        public string AccessToken { get; init; }

        public string RefreshToken { get; set; }
    }
}

namespace FoodApp.Models
{
    public class UserToken
    {
        public Guid id { get; set; }
        public string token { get; set; }
        public DateTime expirationDate { get; set; }
        public bool used { get; set; }
        public Guid userId { get; set; }
        
        public override string ToString()
        {
            return $"Id: {id}, Token: {token}, Expires: {expirationDate}, Used: {used}, UserId: {userId}";
        }
    }

    public class CreateUserTokenRequest
    {
        public string token { get; set; }
        public DateTime expirationDate { get; set; }
        public bool used { get; set; }
        public Guid userId { get; set; }
    }
}
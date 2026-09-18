namespace FoodApp.Models
{
    /// <summary>
    /// Data for refresh_token table
    /// </summary>
    public class RefreshTokenData
    {
        public string? token { get; set; }
        public DateTime? expirationDate { get; set; }
        public bool? used { get; set; }
        public Guid? userId { get; set; }

        public override string ToString()
        {
            return $"Token: {token}, Expires: {expirationDate}, Used: {used}, UserId: {userId}";
        }
    }

    /// <summary>
    /// Full DTO of refresh_token table
    /// </summary>
    public class FullRefreshToken : RefreshTokenData
    {
        public Guid id { get; set; }

        public override string ToString()
        {
            return $"Id: {id}, Token: {token}, Expires: {expirationDate}, Used: {used}, UserId: {userId}";
        }
    }
}

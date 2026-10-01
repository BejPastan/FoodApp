namespace FoodApp.Models
{
    /// <summary>
    /// token model from database
    /// </summary>
    public class UserToken
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        public DateTime expirationDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool used { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid userId { get; set; }
        
        public override string ToString()
        {
            return $"Id: {id}, Token: {token}, Expires: {expirationDate}, Used: {used}, UserId: {userId}";
        }
    }

    /// <summary>
    /// model with data for token when creating user
    /// </summary>
    public class CreateUserTokenRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime expirationDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool used { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid userId { get; set; }
    }
}
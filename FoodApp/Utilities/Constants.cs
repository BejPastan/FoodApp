namespace FoodApp.Utilities
{
    /// <summary>
    /// Constants, not secret values
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Time for auth token to expire
        /// </summary>
        public static int AuthTokenExpire = 900;//15 minutes
        /// <summary>
        /// Time for refresh token to expire
        /// </summary>
        public static int RefreshTokenExpire = 2592000; //1 month

        /// <summary>
        /// Header name for refresh token
        /// </summary>
        public static string refreshHeader = "Refresh-Token";
        /// <summary>
        /// Header name for auth token
        /// </summary>
        public static string authHeader = "AuthToken";
    }
}

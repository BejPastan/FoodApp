namespace FoodApp.Utilities
{
    public class SecretController
    {
        static string serverName = "host.docker.internal\\SQLEXPRESS,1443";
        static string userName = "FoodAppController";
        static string password = "Test@123";

        public static string GetDatabaseCredentials()
        {
            return $"Data Source={serverName};Persist Security Info=False;User ID={userName};Password={password};Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False;Application Name=\"SQL Server Management Studio\";Command Timeout=0";
        }

        static string authSecret = "This is a very secret key for authentication";

        public static string GetAuthSecretKey()
        {
            return authSecret;
        }
    }
}

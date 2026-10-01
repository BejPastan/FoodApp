namespace FoodApp.Utilities
{
    public class SecretController
    {
        static string serverName = "";
        static string userName = "";
        static string password = "";
        static string dbName = "";

        public static void LoadSecrets(ref WebApplicationBuilder builder)
        {
            serverName = builder.Configuration["ConnectionStrings:ServerName"] ?? throw new InvalidOperationException("DB_SERVER configuration is missing.");
            userName = builder.Configuration["ConnectionStrings:UserId"] ?? throw new InvalidOperationException("DB_USER configuration is missing.");
            password = builder.Configuration["ConnectionStrings:Password"] ?? throw new InvalidOperationException("DB_PASSWORD configuration is missing.");
            dbName = builder.Configuration["ConnectionStrings:DatabaseName"] ?? throw new InvalidOperationException("DB_NAME configuration missing");
            authSecret = builder.Configuration["Auth:SecretKey"] ?? throw new InvalidOperationException("AUTH_SECRET_KEY configuration is missing.");
            tokenExpiryMinutes = int.TryParse(builder.Configuration["Auth:TokenExpirationMinutes"], out int minutes) ? minutes : tokenExpiryMinutes;
            emailPass = builder.Configuration["Email:appPassword"] ?? throw new InvalidOperationException("EMAIL_PASSWORD_NOT_FOUND");
            email = builder.Configuration["Email:email"] ?? throw new InvalidOperationException("EMAIL_NOT_FOUND");
        }

        public static string GetDatabaseCredentials()
        {
            return $"Data Source={serverName};Persist Security Info=False;Database={dbName};User ID={userName};Password={password};Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=\"SQL Server Management Studio\";Command Timeout=0";
        }

        static string authSecret = "This is a very secret key for authentication";
        static int tokenExpiryMinutes = 43200;

        public static string GetAuthSecretKey()
        {
            return authSecret;
        }

        static string email;
        static string emailPass;

        public static string GetSmtpEmail()
        {
            return email;
        }

        public static string GetSmtpPass()
        {
            return emailPass;
        }
    }
}

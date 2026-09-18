namespace FoodApp.Models
{
    /// <summary>
    /// Standardized error response model for API errors
    /// </summary>
    public class ErrorResponse
    {
        public string error { get; set; }
        public string? errorCode { get; set; }
        public DateTime timestamp { get; set; }
        public Dictionary<string, string[]>? validationErrors { get; set; }

        public ErrorResponse(string error, string? errorCode = null, Dictionary<string, string[]>? validationErrors = null)
        {
            this.error = error;
            this.errorCode = errorCode;
            timestamp = DateTime.UtcNow;
            this.validationErrors = validationErrors;
        }

        public ErrorResponse(Exception exception, string? errorCode = null)
        {
            error = exception.Message;
            this.errorCode = errorCode;
            timestamp = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Standardized success response model for API responses
    /// </summary>
    public class SuccessResponse(string message ="Success")
    {
        public string message { get; set; } = message;
        public DateTime timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Standardized response for deletion operations
    /// </summary>
    public class DeletionResponse
    {
        public bool deleted { get; set; }
        public string message { get; set; }
        public DateTime timestamp { get; set; }

        public DeletionResponse(bool deleted, string message = "Operation completed")
        {
            this.deleted = deleted;
            this.message = message;
            timestamp = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Standardized response for authentication operations
    /// </summary>
    public class AuthenticationResponse
    {
        /// <summary>
        /// Auth token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// Respons model for auth request
        /// </summary>
        /// <param name="token"></param>
        public AuthenticationResponse(string token)
        {
            this.token = token;
        }
    }
}
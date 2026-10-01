using System;

namespace FoodApp.Models
{
    /// <summary>
    /// Custom exception for validation errors
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ValidationException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ValidationException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Custom exception for not found errors
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public NotFoundException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Custom exception for unauthorized access errors
    /// </summary>
    public class UnauthorizedException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public UnauthorizedException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public UnauthorizedException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Custom exception for business logic violations
    /// </summary>
    public class BusinessLogicException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public BusinessLogicException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public BusinessLogicException(string message, Exception innerException) : base(message, innerException) { }
    }
}

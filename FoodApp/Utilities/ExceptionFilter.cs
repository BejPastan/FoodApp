using FoodApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace FoodApp.Utilities
{
    /// <summary>
    /// Global exception filter to handle and standardize all API errors
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            // Log the exception
            _logger.LogError(context.Exception, "An unhandled exception occurred");

            // Determine the appropriate HTTP status code and error response
            var (statusCode, errorResponse) = MapExceptionToResponse(context.Exception);

            // Set the result
            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = statusCode
            };

            // Mark the exception as handled
            context.ExceptionHandled = true;
        }

        private (int statusCode, ErrorResponse errorResponse) MapExceptionToResponse(Exception exception)
        {
            return exception switch
            {
                ValidationException validationEx => (
                    statusCode: (int)HttpStatusCode.BadRequest,
                    errorResponse: new ErrorResponse(validationEx, "VALIDATION_ERROR")
                ),
                NotFoundException notFoundEx => (
                    statusCode: (int)HttpStatusCode.NotFound,
                    errorResponse: new ErrorResponse(notFoundEx, "NOT_FOUND")
                ),
                UnauthorizedException unauthorizedEx => (
                    statusCode: (int)HttpStatusCode.Unauthorized,
                    errorResponse: new ErrorResponse(unauthorizedEx, "UNAUTHORIZED")
                ),
                BusinessLogicException businessEx => (
                    statusCode: (int)HttpStatusCode.Conflict,
                    errorResponse: new ErrorResponse(businessEx, "BUSINESS_LOGIC_ERROR")
                ),
                UnauthorizedAccessException authEx => (
                    statusCode: (int)HttpStatusCode.Unauthorized,
                    errorResponse: new ErrorResponse(authEx, "UNAUTHORIZED_ACCESS")
                ),
                ArgumentException argEx => (
                    statusCode: (int)HttpStatusCode.BadRequest,
                    errorResponse: new ErrorResponse(argEx, "ARGUMENT_ERROR")
                ),
                _ => (
                    statusCode: (int)HttpStatusCode.InternalServerError,
                    errorResponse: new ErrorResponse("An unexpected error occurred", "INTERNAL_SERVER_ERROR")
                )
            };
        }
    }
}
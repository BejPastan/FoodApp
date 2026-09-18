using FoodApp.Models;
using FoodApp.Services.Interfaces;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    /// <summary>
    /// API controller for managing user authentication and profile information. Provides endpoints for user registration,
    /// login, and retrieving current user information. This controller handles the core authentication functionality
    /// of the FoodApp application.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </remarks>
    /// <param name="service">The user service for data operations.</param>
    [ApiController]
    public class UserController(IUserService service) : ControllerBase
    {
        private readonly IUserService _service = service;

        #region signup
        /// <summary>
        /// Creates a new user account and returns an authentication token.
        /// </summary>
        /// <param name="request">The signup request containing user name, email, and password.</param>
        /// <returns>An authentication token for the newly created user.</returns>
        /// <response code="200">Returns the authentication token for the new user.</response>
        /// <response code="400">Bad Request - invalid signup data or missing required fields.</response>
        /// <response code="409">Conflict - a user with this email already exists.</response>
        /// <remarks>
        /// Creates a new user account with the provided name, email, and password.
        /// The password is securely hashed before storage. The email must be unique within the system.
        /// Upon successful registration, a JWT authentication token is returned that can be used for subsequent API calls.
        /// The token has a limited expiry time and should be refreshed using the /api/auth/me endpoint.
        /// </remarks>
        [HttpPost("api/users/signup")]
        public IActionResult SignUp([FromBody] SignUpRequest request)
        {
            var succcess = _service.SignUpUser(request.name, request.email, request.password);
            SuccessResponse resp = new()
            {
                message = "Account created"
            };
            return Ok(resp);
        }


        /// <summary>
        /// Confirms user account registration using verification token.
        /// </summary>
        /// <param name="request">Request containing confirmation token.</param>
        /// <returns>Success status of account confirmation.</returns>
        /// <response code="200">Returns success status if account was confirmed.</response>
        /// <response code="400">Bad Request - invalid token.</response>
        /// <response code="404">Not Found - invalid or expired token.</response>
        /// <remarks>
        /// Public endpoint. Validates the registration confirmation token and activates the user account.
        /// The token is marked as used after this operation.
        /// </remarks>
        [HttpPost("api/users/signup/confirm")]
        public IActionResult ConfirmSignUp([FromBody] ConfirmSignUpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.token))
            {
                return BadRequest(new { error = "Token is required" });
            }

            var success = _service.ConfirmSignUp(request.token);
            if (!success)
            {
                return NotFound(new { error = "Invalid or expired token" });
            }

            return Ok(new { success = true });
        }

        #endregion

        #region Loggin cycle
        /// <summary>
        /// Authenticates a user and returns auth and refresh token
        /// </summary>
        /// <param name="request">The login request containing email and password.</param>
        /// <returns>An authentication token for the authenticated user.</returns>
        /// <response code="200">Returns the authentication token.</response>
        /// <response code="400">Bad Request - invalid login data or missing required fields.</response>
        /// <response code="401">Unauthorized - invalid email or password.</response>
        /// <remarks>
        /// Authenticates the user with the provided email and password credentials.
        /// The password is verified against the stored hash. Upon successful authentication,
        /// a JWT token is returned that should be included in the Authorization header for subsequent requests.
        /// The token includes the user's ID and has a limited expiry time.
        /// </remarks>
        [HttpPost("api/auth/login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = _service.LoginUser(request.email, request.password);
                Response.Cookies.Append(Constants.authHeader,token.authToken, authCookie);
                Response.Cookies.Append(Constants.refreshHeader, token.refreshToken, refreshCookie);
                return Ok(new SuccessResponse());
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex);
                throw;
            }

        }

        /// <summary>
        /// Retrieves the current user's profile information
        /// </summary>
        /// <returns>The current user's profile information including role details.</returns>
        /// <response code="200">Returns the current user's profile with a refreshed token.</response>
        /// <response code="401">Unauthorized - invalid or expired authentication token.</response>
        /// <response code="404">Not Found - user account no longer exists.</response>
        /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated.</exception>
        [HttpGet("api/auth/me")]
        public IActionResult GetCurrentUser()
        {
            Guid? userId = Authentication.GetUserIdFromHeader(Request) ?? throw new UnauthorizedAccessException("You don't have permission to do this");
            Console.WriteLine(userId);
            
            ExtendedUser? user = _service.GetCurrentUser(userId.Value);
            return Ok(user);
        }

        /// <summary>
        /// exchange refresh token for auth token
        /// </summary>
        /// <returns>return empty response with header with new Auth token, and refresh token</returns>
        [HttpGet("api/auth/refresh")]
        public IActionResult RefreshToken()
        {
            try
            {
                //int? userId = Authentication.GetUserIdFromHeader(Request) ?? throw new UnauthorizedAccessException("You don't have permission to do this");
                var token = _service.RefreshAuthToken(Request.Cookies[Constants.refreshHeader]);

                Response.Cookies.Append(Constants.authHeader, token.authToken, authCookie);
                Response.Cookies.Append(Constants.refreshHeader, token.refreshToken, refreshCookie);
                return Ok(new SuccessResponse());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpPost("api/auth/logout")]
        public IActionResult Logout()
        {
            Guid? userId = Authentication.GetUserIdFromHeader(Request) ?? throw new UnauthorizedAccessException("You don't have permission to do this");
            _service.Logout(userId.Value);
            return Ok(new SuccessResponse());
        }


        #endregion
        #region password reset
        /// <summary>
        /// Initiates password reset process for a user.
        /// </summary>
        /// <param name="request">Request containing the user's email address.</param>
        /// <returns>Success status regardless of whether email exists</returns>
        /// <response code="200">Returns success status.</response>
        /// <response code="400">Bad Request - invalid email format.</response>
        /// <remarks>
        /// Public endpoint. If the email exists in the system, a password reset token will be generated 
        /// and sent via email. Always returns success even for non-existing emails
        /// </remarks>
        [HttpPost("api/users/reset-password")]
        public IActionResult StartPasswordReset([FromBody] PasswordResetStartRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.email))
            {
                return BadRequest(new { error = "Email is required" });
            }

            _service.StartPasswordReset(request.email);
            return Ok(new { success = true });
        }

        /// <summary>
        /// Completes password reset process using verification token.
        /// </summary>
        /// <param name="request">Request containing reset token and new password.</param>
        /// <returns>Success status of password reset operation.</returns>
        /// <response code="200">Returns success status if password was reset.</response>
        /// <response code="400">Bad Request - invalid token or password.</response>
        /// <response code="404">Not Found - invalid or expired token.</response>
        /// <remarks>
        /// Public endpoint. Validates the reset token and updates the user's password if the token is valid.
        /// The token is marked as used after this operation and cannot be reused.
        /// </remarks>
        [HttpPost("api/users/reset-password/confirm")]
        public IActionResult ConfirmPasswordReset([FromBody] PasswordResetConfirmRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.token) || string.IsNullOrWhiteSpace(request.newPassword))
            {
                return BadRequest(new { error = "Token and new password are required" });
            }

            var success = _service.ConfirmPasswordReset(request.token, request.newPassword);
            if (!success)
            {
                return NotFound(new { error = "Invalid or expired token" });
            }

            return Ok(new { success = true });
        }
        #endregion

        /// <summary>
        /// Updates the authenticated user's profile information.
        /// </summary>
        /// <param name="request">Request containing updated profile fields.</param>
        /// <returns>The updated user profile information.</returns>
        /// <response code="200">Returns the updated user profile.</response>
        /// <response code="400">Bad Request - invalid update data.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="409">Conflict - email address is already in use.</response>
        /// <remarks>
        /// Requires valid authentication token. Users can only update their own profile.
        /// Supports partial updates - only provided fields will be modified.
        /// </remarks>
        [HttpPatch("api/users/profile")]
        public IActionResult UpdateUserProfile([FromBody] UserUpdateRequest request)
        {
            Guid? userId = Authentication.GetUserIdFromHeader(Request) ?? throw new UnauthorizedAccessException("You don't have permission to do this");
            var updatedUser = _service.UpdateUser(request, userId.Value);
            return Ok(updatedUser);
        }

        /// <summary>
        /// Delete user data from database
        /// </summary>
        /// <param name="userId">id of user to delete</param>
        /// <returns>when successfull return message</returns>
        [HttpPost("api/users/delete/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            return Ok(new SuccessResponse(message: "user removed"));
        }

        private readonly CookieOptions authCookie =
        new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddSeconds(Constants.AuthTokenExpire)
        };

        private readonly CookieOptions refreshCookie = 
        new(){
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddSeconds(Constants.RefreshTokenExpire),
            Path = "/api/auth/refresh"
        };
    }
}

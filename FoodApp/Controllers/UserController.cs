using FoodApp.Models;
using FoodApp.Repositories;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FoodApp.Controllers
{
    /// <summary>
    /// API controller for managing user authentication and profile information. Provides endpoints for user registration,
    /// login, and retrieving current user information. This controller handles the core authentication functionality
    /// of the FoodApp application.
    /// </summary>
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IAuthService _auth;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="service">The user service for data operations.</param>
        /// <param name="authService">The authentication service for permission checks.</param>
        public UserController(IUserService service, IAuthService authService)
        {
            _service = service;
            _auth = authService;
        }

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
            var token = _service.SignUpUser(request.name, request.email, request.password);
            AuthenticationResponse resp = new AuthenticationResponse(token);
            return Ok(resp);
        }

        /// <summary>
        /// Authenticates a user and returns an authentication token.
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
        [HttpPost("api/users/login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var token = _service.LoginUser(request.email, request.password);
            AuthenticationResponse resp = new AuthenticationResponse(token);
            return Ok(resp);
        }

        /// <summary>
        /// Retrieves the current user's profile information and refreshes the authentication token.
        /// </summary>
        /// <returns>The current user's profile information including role details.</returns>
        /// <response code="200">Returns the current user's profile with a refreshed token.</response>
        /// <response code="401">Unauthorized - invalid or expired authentication token.</response>
        /// <response code="404">Not Found - user account no longer exists.</response>
        /// <remarks>
        /// Requires valid authentication token in the Authorization header. This endpoint serves multiple purposes:
        /// 1. Validates that the current token is still valid
        /// 2. Returns the current user's profile information including their role
        /// 3. Issues a new token with extended expiry time in the response headers
        /// The new token should be stored by the client and used for subsequent API calls.
        /// This endpoint is typically called when the client detects that the token is about to expire.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated.</exception>
        [HttpGet("api/auth/me")]
        public IActionResult GetCurrentUser()
        {
            int? userId = Authentication.GetUserIdFromHeader(Request);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("You don't have permission to do this");
            }

            ExtendedUser user = _service.GetCurrentUser(userId.Value);
            string token = Authentication.CreateAuthToken(userId.Value);
            Response.Headers.Add("new-token", token);
            return Ok(user);
        }
    }
}

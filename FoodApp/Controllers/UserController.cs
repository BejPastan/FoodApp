using FoodApp.Models;
using FoodApp.Repositories;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FoodApp.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IAuthService _auth;
        public UserController(IUserService service, IAuthService authService)
        {
            _service = service;
            _auth = authService;
        }


        /// <summary>
        /// create new user and return auth token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// Create new user and return token
        /// </remarks>
        /// <response code="200">Success</response>
        [HttpPost("api/users/signup")]
        public IActionResult SignUp([FromBody] SignUpRequest request)
        {
            var token = _service.SignUpUser(request.name, request.email, request.password);
            AuthenticationResponse resp = new AuthenticationResponse(token);
            return Ok(resp);
        }

        [HttpPost("api/users/login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var token = _service.LoginUser(request.email, request.password);
            AuthenticationResponse resp = new AuthenticationResponse(token);
            return Ok(resp);
        }

        /// <summary>
        /// get user belonging to token, and return new token with extended expiry time
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
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

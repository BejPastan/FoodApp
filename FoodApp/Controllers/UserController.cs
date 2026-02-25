using FoodApp.Models;
using FoodApp.Repositories;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost("api/users/signup")]
        public IActionResult SignUp([FromBody] SignUpRequest request)
        {
            

            Console.WriteLine($"email: {request.email}, password: {request.password}, name:{request.name}");
            try
            {
                var token = _service.SignUpUser(request.name, request.email, request.password);
                Console.WriteLine($"token {new {token = token}}");
                return Ok(new { token = token });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("api/users/login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = _service.LoginUser(request.email, request.password);
                Console.WriteLine($"token {new { token = token }}");
                return Ok(new { token = token });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("api/auth/me")]
        public IActionResult GetCurrentUser([FromHeader(Name = "Authorization")] string authorization)
        {
            Console.WriteLine(authorization);

            try
            {
                var user = _service.GetCurrentUser(authorization);
                if (user == null)
                {
                    return Unauthorized(new { error = "Invalid authorization header or token" });
                }
                Console.WriteLine(user);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}

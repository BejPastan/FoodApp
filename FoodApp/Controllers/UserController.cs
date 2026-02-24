using Microsoft.AspNetCore.Mvc;
using FoodApp.Repositories;
using FoodApp.Utilities;
using FoodApp.Services;

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
        public IActionResult SignUp([FromQuery] string email, [FromQuery] string password, [FromQuery] string name)
        {
            try
            {
                var token = _service.SignUpUser(name, email, password);
                return Ok(new { token });
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

        [HttpPost("api/users/login")]
        public IActionResult Login([FromQuery] string email, [FromQuery] string password)
        {
            try
            {
                var token = _service.LoginUser(email, password);
                return Ok(new { token });
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

        [HttpGet("auth/me")]
        public IActionResult GetCurrentUser([FromHeader(Name = "Authorization")] string authorization)
        {
            try
            {
                var user = _service.GetCurrentUser(authorization);
                if (user == null)
                {
                    return Unauthorized(new { error = "Invalid authorization header or token" });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}

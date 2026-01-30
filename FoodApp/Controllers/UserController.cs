using Microsoft.AspNetCore.Mvc;
using FoodApp.Repositories;
using FoodApp.Utilities;

namespace FoodApp.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repo;
        public UserController(IUserRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("api/users/signup")]
        public IActionResult SignUp([FromQuery] string email, [FromQuery] string password, [FromQuery] string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest(new { error = "email, password and name are required" });
                }
                var hashed = Authentication.HashPassword(password);
                var user = _repo.SignUpUser(name, hashed, email);
                var token = Authentication.CreateAuthToken(user.id);
                return Ok(new { token });
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
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return BadRequest(new { error = "email and password are required" });
                }
                var user = _repo.GetUserByEmail(email);
                if (user == null || !Authentication.CheckPassword(password, user.password))
                {
                    return Unauthorized(new { error = "invalid email or password" });
                }
                user = _repo.LoginUser(user.id);
                var token = Authentication.CreateAuthToken(user.id);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}

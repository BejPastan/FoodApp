using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    [ApiController]
    public class UserMealController : Controller
    {
        private readonly IUserMealService _service;
        public UserMealController(IUserMealService service) { _service = service; }

        [HttpGet("api/user_meals")]
        
        public IActionResult GetUserMeals([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var userId = Authentication.GetUserIdFromHeader(Request);
            if (userId == null)
            {
                throw new UnauthorizedAccessException("You don't have permission to do this");
            }
            return Ok(_service.GetUserMeals(userId.Value, startDate, endDate));
        }

        [HttpGet("api/user_meals/{id}")]
        public IActionResult GetUserMeal(int id)
        {
                var item = _service.GetUserMealById(id); 
                if (item == null) return NotFound(); 
                return Ok(item);
        }

        [HttpPost("api/user_meals")]
        public IActionResult PostUserMeal([FromBody] UserMeal request)
        {
                var userId = Authentication.GetUserIdFromHeader(Request);
                if(userId==null)
                {
                    return Unauthorized(new { error = "You don't have permission to do this" });
                }
                request.userId = userId.Value;
                var created = _service.CreateUserMeal(request); 
                return Ok(created);
        }

        [HttpDelete("api/user_meals/{id}")]
        public IActionResult DeleteUserMeal(int id)
        {
            _service.DeleteUserMeal(id); return Ok(new { deleted = true });
        }
    }
}

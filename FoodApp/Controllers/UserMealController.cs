using Microsoft.AspNetCore.Mvc;
using FoodApp.Services;
using FoodApp.Models;

namespace FoodApp.Controllers
{
    [ApiController]
    public class UserMealController : Controller
    {
        private readonly IUserMealService _service;
        public UserMealController(IUserMealService service) { _service = service; }

        [HttpGet("api/user_meals")]
        public IActionResult GetUserMeals([FromQuery] int userId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try { return Ok(_service.GetUserMeals(userId, startDate, endDate)); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpGet("api/user_meals/{id}")]
        public IActionResult GetUserMeal(int id)
        {
            try { var item = _service.GetUserMealById(id); if (item == null) return NotFound(); return Ok(item); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpPost("api/user_meals")]
        public IActionResult PostUserMeal([FromBody] UserMeal request)
        {
            try { var created = _service.CreateUserMeal(request); return Ok(created); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpDelete("api/user_meals/{id}")]
        public IActionResult DeleteUserMeal(int id)
        {
            try { _service.DeleteUserMeal(id); return Ok(new { deleted = true }); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }
    }
}

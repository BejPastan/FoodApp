using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    [ApiController]
    public class MealController : Controller
    {
        private readonly IMealService _service;
        private readonly IAuthService _auth;
        public MealController(IMealService service, IAuthService authService)
        {
            _service = service;
            _auth = authService;
        }

        [HttpGet("api/meals")]
        public IActionResult GetMeals([FromQuery] string name = "", [FromQuery] int page = 1, [FromQuery] int perPage = 25)
        {
                Authentication.ValidateToken(Request);
                var response = _service.GetMeals(name, null, page, perPage);
                Console.WriteLine(response.Length);
                return Ok(response);
        }

        [HttpGet("api/meals/{id}")]
        public IActionResult GetMeal(int id)
        {
            Authentication.ValidateToken(Request);
                var item = _service.GetMealById(id);
                if (item == null)
                {
                    return NotFound();
                }
                return Ok(item); 
        }

        [HttpPost("api/meals")]
        public IActionResult PostMeal([FromBody] Meal mealRequest)
        {
                _auth.CheckPermissions(Request, [Roles.admin]);
                var created = _service.CreateMeal(mealRequest);
                return Ok(created);
        }

        [HttpPatch("api/meals/{id}")]
        public IActionResult PatchMeal(int id, [FromQuery] string? name = null)
        {
                var userId = _auth.CheckPermissions(Request, [Roles.admin]);
                var updated = _service.UpdateMeal(id, name);
                if (updated == null) return BadRequest(new { error = "No fields or not found" });
                return Ok(updated);
        }

        [HttpDelete("api/meals/{id}")]
        public IActionResult DeleteMeal(int id)
        {
                var userId = _auth.CheckPermissions(Request, [Roles.admin]);
                _service.DeleteMeal(id);
                return Ok(new { deleted = true });
        }
    }
}

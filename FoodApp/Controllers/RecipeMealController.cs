using Microsoft.AspNetCore.Mvc;
using FoodApp.Services;
using FoodApp.Models;

namespace FoodApp.Controllers
{
    [ApiController]
    public class RecipeMealController : Controller
    {
        private readonly IRecipeMealService _service;
        public RecipeMealController(IRecipeMealService service) { _service = service; }

        [HttpGet("api/recipe_meals")]
        public IActionResult GetRecipeMeals([FromQuery] int? recipeId = null, [FromQuery] int? mealId = null)
        {
            try { return Ok(_service.GetRecipeMeals(recipeId, mealId)); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpGet("api/recipe_meals/{id}")]
        public IActionResult GetRecipeMeal(int id)
        {
            try { var item = _service.GetRecipeMealById(id); if (item == null) return NotFound(); return Ok(item); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpPost("api/recipe_meals")]
        public IActionResult PostRecipeMeal([FromBody] RecipeMeal request)
        {
            try { var created = _service.CreateRecipeMeal(request); return Ok(created); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpDelete("api/recipe_meals/{id}")]
        public IActionResult DeleteRecipeMeal(int id)
        {
            try { _service.DeleteRecipeMeal(id); return Ok(new { deleted = true }); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }
    }
}

using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    [ApiController]
    public class RecipeMealController : Controller
    {
        private readonly IRecipeMealService _service;
        private readonly IAuthService _auth;
        public RecipeMealController(IRecipeMealService service, IAuthService auth)
        { 
            _service = service;
            _auth = auth;
        }

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
        public IActionResult PostRecipeMeal([FromBody] RecipeMeal request, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);

            try { var created = _service.CreateRecipeMeal(request); return Ok(created); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpDelete("api/recipe_meals/{id}")]
        public IActionResult DeleteRecipeMeal(int id, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);

            try { _service.DeleteRecipeMeal(id); return Ok(new { deleted = true }); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }
    }
}

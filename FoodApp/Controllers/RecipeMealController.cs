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
        public RecipeMealController(IRecipeMealService service, IAuthService authService)
        {
            _service = service;
            _auth = authService;
        }

        [HttpGet("api/recipe_meals")]
        public IActionResult GetRecipeMeals([FromQuery] int? recipeId = null, [FromQuery] int? mealId = null)
        {
            Authentication.ValidateToken(Request);
            return Ok(_service.GetRecipeMeals(recipeId, mealId));
        }

        [HttpGet("api/recipe_meals/{id}")]
        public IActionResult GetRecipeMeal(int id)
        {
            Authentication.ValidateToken(Request);
            var item = _service.GetRecipeMealById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost("api/recipe_meals")]
        public IActionResult PostRecipeMeal([FromBody] RecipeMeal request)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            var created = _service.CreateRecipeMeal(request);
            return Ok(created);
        }

        [HttpDelete("api/recipe_meals/{id}")]
        public IActionResult DeleteRecipeMeal(int id)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            _service.DeleteRecipeMeal(id);
            return Ok(new { deleted = true });
        }
    }
}

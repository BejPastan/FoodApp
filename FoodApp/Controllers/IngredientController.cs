using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    [ApiController]
    public class IngredientController : Controller
    {
        private readonly IIngredientService _service;
        private readonly IAuthService _auth;
        public IngredientController(IIngredientService service, IAuthService authService)
        {
            _service = service;
            _auth = authService;
        }

        [HttpGet("api/ingredients")]
        public IActionResult GetIngredients([FromQuery] int? recipeId = null, [FromQuery] int? foodId = null, [FromQuery] int page = 1, [FromQuery]int perPage = 25)
        {
                Authentication.ValidateToken(Request);
                Ingredient[] ingredients = _service.GetIngredients(recipeId, foodId,page, perPage).ToArray();
                return Ok(ingredients);
        }

        [HttpGet("api/ingredients/{id}")]
        public IActionResult GetIngredient(int id)
        {
                Authentication.ValidateToken(Request);
                var item = _service.GetIngredientById(id);
                if (item == null)
                {
                    return NotFound();
                }
                return Ok(item);
        }

        [HttpPost("api/ingredients")]
        public IActionResult PostIngredient([FromBody] Ingredient request, [FromHeader(Name = "Authorization")] string authorization)
        {
                var userId = _auth.CheckPermissions(Request, [Roles.admin]);
                var created = _service.CreateIngredient(request);
                return Ok(created);
        }

        [HttpPatch("api/ingredients/{id}")]
        public IActionResult PatchIngredient(int id, [FromBody] Ingredient ingredient, [FromHeader(Name = "Authorization")] string authorization)
        {
                var userId = _auth.CheckPermissions(Request, [Roles.admin]);
                var updated = _service.UpdateIngredient(ingredient);
                if (updated == null)
                {
                    return BadRequest(new { error = "No fields or not found" });
                }
                return Ok(updated);
        }

        [HttpDelete("api/ingredients/{id}")]
        public IActionResult DeleteIngredient(int id, [FromHeader(Name = "Authorization")] string authorization)
        {
                var userId = _auth.CheckPermissions(Request, [Roles.admin]);
                _service.DeleteIngredient(id);
                return Ok(new { deleted = true });
        }
    }
}

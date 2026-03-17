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
        public IngredientController(IIngredientService service, IAuthService auth)
        {
            _service = service;
            _auth = auth;
        }

        [HttpGet("api/ingredients")]
        public IActionResult GetIngredients([FromQuery] int? recipeId = null, [FromQuery] int? foodId = null, [FromQuery] int page = 1, [FromQuery]int perPage = 25)
        {
            try
            {
                Ingredient[] ingredients = _service.GetIngredients(recipeId, foodId,page, perPage).ToArray();
                return Ok(ingredients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("api/ingredients/{id}")]
        public IActionResult GetIngredient(int id)
        {
            try
            {
                var item = _service.GetIngredientById(id);
                if (item == null)
                {
                    return NotFound();
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("api/ingredients")]
        public IActionResult PostIngredient([FromBody] Ingredient request, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);
            try 
            { 
                var created = _service.CreateIngredient(request); 
                return Ok(created); 
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, new { error = ex.Message }); 
            }
        }

        [HttpPatch("api/ingredients/{id}")]
        public IActionResult PatchIngredient(int id, [FromBody] Ingredient ingredient, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);

            try 
            {
                var updated = _service.UpdateIngredient(ingredient);
                if (updated == null)
                {
                    return BadRequest(new { error = "No fields or not found" });
                }
                return Ok(updated);
            }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpDelete("api/ingredients/{id}")]
        public IActionResult DeleteIngredient(int id, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);
            try { _service.DeleteIngredient(id); return Ok(new { deleted = true }); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }
    }
}

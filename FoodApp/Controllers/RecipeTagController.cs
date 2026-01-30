using Microsoft.AspNetCore.Mvc;
using FoodApp.Services;
using FoodApp.Models;

namespace FoodApp.Controllers
{
    [ApiController]
    public class RecipeTagController : Controller
    {
        private readonly IRecipeTagService _service;
        public RecipeTagController(IRecipeTagService service) { _service = service; }

        [HttpGet("api/recipe_tags")]
        public IActionResult GetRecipeTags([FromQuery] int? recipeId = null, [FromQuery] int? tagId = null)
        {
            try { return Ok(_service.GetRecipeTags(recipeId, tagId)); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpPost("api/recipe_tags")]
        public IActionResult PostRecipeTag([FromBody] RecipeTag request)
        {
            try { var created = _service.CreateRecipeTag(request); return Ok(created); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpDelete("api/recipe_tags")]
        public IActionResult DeleteRecipeTag([FromQuery] int recipeId, [FromQuery] int tagId)
        {
            try { _service.DeleteRecipeTag(recipeId, tagId); return Ok(new { deleted = true }); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }
    }
}

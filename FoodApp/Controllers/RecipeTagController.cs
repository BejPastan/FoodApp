using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    [ApiController]
    public class RecipeTagController : Controller
    {
        private readonly IRecipeTagService _service;
        private readonly IAuthService _auth;
        public RecipeTagController(IRecipeTagService service, IAuthService authService)
        {
            _service = service;
            _auth = authService;
        }

        [HttpGet("api/recipe_tags")]
        public IActionResult GetRecipeTags([FromQuery] int? recipeId = null, [FromQuery] int? tagId = null)
        {
            Authentication.ValidateToken(Request);
            return Ok(_service.GetRecipeTags(recipeId, tagId));
        }

        [HttpPost("api/recipe_tags")]
        public IActionResult PostRecipeTag([FromBody] RecipeTag request)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            var created = _service.CreateRecipeTag(request);
            return Ok(created);
        }

        [HttpDelete("api/recipe_tags")]
        public IActionResult DeleteRecipeTag([FromQuery] int recipeId, [FromQuery] int tagId)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            _service.DeleteRecipeTag(recipeId, tagId);
            return Ok(new { deleted = true });
        }
    }
}

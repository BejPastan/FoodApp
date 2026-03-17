using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FoodApp.Controllers
{
    [ApiController]
    public class RecipeTagController : Controller
    {
        private readonly IRecipeTagService _service;
        private readonly IAuthService _auth;
        public RecipeTagController(IRecipeTagService service, IAuthService auth) 
        {
            _auth = auth;
            _service = service; 
        }

        [HttpGet("api/recipe_tags")]
        public IActionResult GetRecipeTags([FromQuery] int? recipeId = null, [FromQuery] int? tagId = null)
        {
            try { return Ok(_service.GetRecipeTags(recipeId, tagId)); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpPost("api/recipe_tags")]
        public IActionResult PostRecipeTag([FromBody] RecipeTag request, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);

            try { var created = _service.CreateRecipeTag(request); return Ok(created); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpDelete("api/recipe_tags")]
        public IActionResult DeleteRecipeTag([FromQuery] int recipeId, [FromQuery] int tagId, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);

            try { _service.DeleteRecipeTag(recipeId, tagId); return Ok(new { deleted = true }); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }
    }
}

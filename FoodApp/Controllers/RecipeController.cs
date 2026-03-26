using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    [ApiController]
    public class RecipeController : Controller
    {
        private const int CHOOSE_SIZE = 3;


        private readonly IRecipeService _service;
        private readonly IAuthService _auth;

        public RecipeController(IRecipeService service, IAuthService authService)
        {
            _service = service;
            _auth = authService;
        }

        [HttpGet("api/recipes")]
        public IActionResult GetRecipesAPI([FromQuery] string name = "", [FromQuery] int page = 1, [FromQuery] int perPage = 25)
        {

                Authentication.ValidateToken(Request);
                return Ok(_service.GetRecipes(name, page, perPage));

        }

        [HttpGet("api/recipes/{id}")]
        public IActionResult GetRecipe(int id)
        {

                Authentication.ValidateToken(Request);
                Recipe? item = _service.GetRecipeById(id);
                if (item == null)
                {
                    return NotFound();
                }
                Console.WriteLine($"found recipe: {item.name}");
                return Ok(item);

        }

        [HttpPost("api/recipes")]
        public IActionResult PostRecipe([FromBody] Recipe request, [FromHeader(Name = "Authorization")] string authorization)
        {

                var userId = _auth.CheckPermissions(Request, [Roles.admin]);
                var created = _service.CreateRecipe(request);
                return Ok(created);

        }

        [HttpPatch("api/recipes/{id}")]
        public IActionResult PatchRecipe(int id, [FromBody] Recipe request, [FromHeader(Name = "Authorization")] string authorization)
        {

                var userId = _auth.CheckPermissions(Request, [Roles.admin]);
                var updated = _service.UpdateRecipe(request);
                if (updated == null)
                {
                    return BadRequest(new { error = "No fields or not found" });
                }
                return Ok(updated);

        }

        [HttpDelete("api/recipes/{id}")]
        public IActionResult DeleteRecipe(int id, [FromHeader(Name = "Authorization")] string authorization)
        {

                var userId = _auth.CheckPermissions(Request, [Roles.admin]);
                _service.DeleteRecipe(id);
                return Ok(new { deleted = true });

        }

        [HttpGet("api/recipes/choices")]
        public IActionResult GetRecipeToChoose([FromQuery] int mealId, [FromQuery] int excludeWeeks)
        {

                int? userId = Authentication.GetUserIdFromHeader(Request);
                if(userId==null)
                {
                    throw new UnauthorizedAccessException("You are not authorized to do this");
                }
                Recipe[] items = _service.GetRecipeToChoose(mealId, userId.Value, excludeWeeks, CHOOSE_SIZE);
                return Ok(items);

        }
    }
}

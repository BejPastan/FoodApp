using Microsoft.AspNetCore.Mvc;
using FoodApp.Services;
using FoodApp.Models;

namespace FoodApp.Controllers
{
    [ApiController]
    public class RecipeController : Controller
    {
        private const int CHOOSE_SIZE = 3;


        private readonly IRecipeService _service;
        public RecipeController(IRecipeService service)
        {
            _service = service;
        }

        [HttpGet("api/recipes")]
        public IActionResult GetRecipesAPI([FromQuery] string name = "", [FromQuery] int page = 1, [FromQuery] int perPage = 25)
        {
            Console.WriteLine($"searching recipes with name {name}");
            try
            {
                return Ok(_service.GetRecipes(name, page, perPage));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("api/recipes/{id}")]
        public IActionResult GetRecipe(int id)
        {
            try
            {
                Recipe? item = _service.GetRecipeById(id);
                if (item == null)
                {
                    return NotFound();
                }
                Console.WriteLine($"found recipe: {item.name}");
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("api/recipes")]
        public IActionResult PostRecipe([FromBody] Recipe request)
        {
            try 
            { 
                var created = _service.CreateRecipe(request); return Ok(created); 
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, new { error = ex.Message }); 
            }
        }

        [HttpPatch("api/recipes/{id}")]
        public IActionResult PatchRecipe(int id, [FromBody] Recipe request)
        {
            try 
            { 
                var updated = _service.UpdateRecipe(request);
                if (updated == null)
                {
                    return BadRequest(new { error = "No fields or not found" }); 
                }
                return Ok(updated);
            }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpDelete("api/recipes/{id}")]
        public IActionResult DeleteRecipe(int id)
        {
            try { _service.DeleteRecipe(id); return Ok(new { deleted = true }); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpGet("api/recipes/choices")]
        public IActionResult GetRecipeToChoose([FromQuery] int mealId, [FromQuery] int userId, [FromQuery] int excludeWeeks)
        {
            try
            {
                Recipe[] items = _service.GetRecipeToChoose(mealId, userId, excludeWeeks, CHOOSE_SIZE);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });

            }
        }
    }
}

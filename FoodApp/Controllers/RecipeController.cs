using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    /// <summary>
    /// API controller for managing recipes. Provides comprehensive endpoints for recipe CRUD operations,
    /// including ingredients, steps, and meal associations. Also provides functionality for recipe selection
    /// to avoid repetition in meal planning.
    /// </summary>
    [ApiController]
    public class RecipeController : Controller
    {
        private const int CHOOSE_SIZE = 3;

        private readonly IRecipeService _service;
        private readonly IAuthService _auth;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeController"/> class.
        /// </summary>
        /// <param name="service">The recipe service for data operations.</param>
        /// <param name="authService">The authentication service for permission checks.</param>
        public RecipeController(IRecipeService service, IAuthService authService)
        {
            _service = service;
            _auth = authService;
        }

        /// <summary>
        /// Retrieves a list of recipes with optional filtering and pagination.
        /// </summary>
        /// <param name="name">Optional name to search for (partial match, case-insensitive).</param>
        /// <param name="page">Page number for pagination (default: 1).</param>
        /// <param name="perPage">Number of items per page (default: 25, max: 100).</param>
        /// <returns>A list of recipes matching the search criteria with full details including ingredients, steps, and associated meals.</returns>
        /// <response code="200">Returns the list of matching recipes.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <remarks>
        /// Requires user authentication. Supports pagination with page and perPage parameters.
        /// If name is provided, recipes containing that text in their name are returned.
        /// Each recipe includes complete information about its ingredients, preparation steps, and meal associations.
        /// Results are sorted by recipe ID in ascending order.
        /// </remarks>
        [HttpGet("api/recipes")]
        public IActionResult GetRecipesAPI([FromQuery] string name = "", [FromQuery] int page = 1, [FromQuery] int perPage = 25)
        {
                Authentication.ValidateToken(Request);
                return Ok(_service.GetRecipes(name, page, perPage));
        }

        /// <summary>
        /// Retrieves a specific recipe by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the recipe.</param>
        /// <returns>The recipe with complete details including ingredients, steps, and associated meals if found; otherwise returns 404 Not Found.</returns>
        /// <response code="200">Returns the requested recipe with full details.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="404">Not Found - recipe with the specified ID does not exist.</response>
        /// <remarks>
        /// Requires user authentication. Returns complete recipe information including:
        /// - Basic recipe details (name, preparation time, portions)
        /// - Complete ingredient list with quantities and units
        /// - Step-by-step preparation instructions
        /// - Associated meal types
        /// </remarks>
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

        /// <summary>
        /// Creates a new recipe with complete details.
        /// </summary>
        /// <param name="request">The recipe data to create including name, preparation time, portions, ingredients, steps, and meal associations.</param>
        /// <returns>The created recipe with its assigned ID and complete details.</returns>
        /// <response code="200">Returns the created recipe.</response>
        /// <response code="400">Bad Request - invalid recipe data or missing required fields.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. The recipe must include:
        /// - Name (required, unique within the system)
        /// - Preparation time in minutes (required, positive integer)
        /// - Number of portions (required, positive integer)
        /// - Complete ingredient list with valid food IDs, unit IDs, and quantities
        /// - Step-by-step instructions in order
        /// - Associated meal types
        /// All referenced IDs must exist in the database.
        /// </remarks>
        [HttpPost("api/recipes")]
        public IActionResult PostRecipe([FromBody] Recipe request)
        {
                var userId = _auth.CheckPermissions(Request, [Roles.admin]);
                var created = _service.CreateRecipe(request);
                return Ok(created);
        }

        /// <summary>
        /// Updates an existing recipe.
        /// </summary>
        /// <param name="id">The ID of the recipe to update (must match the recipe's ID in the request body).</param>
        /// <param name="request">The updated recipe data. The ID in the body must match the route parameter.</param>
        /// <returns>The updated recipe, or 400 Bad Request if no valid fields to update or recipe not found.</returns>
        /// <response code="200">Returns the updated recipe.</response>
        /// <response code="400">Bad Request - no valid fields provided for update or recipe not found.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. The recipe object must have its ID property set to match the route parameter.
        /// All recipe components can be updated including:
        /// - Basic recipe details (name, preparation time, portions)
        /// - Complete ingredient list (add, update, or remove ingredients)
        /// - Preparation steps (add, update, or reorder steps)
        /// - Meal associations (add or remove meal types)
        /// Returns 400 if no fields are provided for update.
        /// All referenced IDs must remain valid after the update.
        /// </remarks>
        [HttpPatch("api/recipes/{id}")]
        public IActionResult PatchRecipe(int id, [FromBody] Recipe request)
        {
                var userId = _auth.CheckPermissions(Request, [Roles.admin]);
                var updated = _service.UpdateRecipe(request);
                if (updated == null)
                {
                    return BadRequest(new { error = "No fields or not found" });
                }
                return Ok(updated);
        }

        /// <summary>
        /// Deletes a recipe by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the recipe to delete.</param>
        /// <returns>Success status of the deletion operation.</returns>
        /// <response code="200">Returns deletion status (true if successful, false if not found).</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. This operation performs a cascading deletion:
        /// - The recipe is permanently removed from the database
        /// - All associated ingredients are removed
        /// - All associated steps are removed
        /// - All recipe-meal associations are removed
        /// - All recipe-tag associations are removed
        /// - All user meal records referencing this recipe remain but lose the recipe reference
        /// This is a destructive operation that cannot be undone.
        /// </remarks>
        [HttpDelete("api/recipes/{id}")]
        public IActionResult DeleteRecipe(int id)
        {
                var userId = _auth.CheckPermissions(Request, [Roles.admin]);
                _service.DeleteRecipe(id);
                return Ok(new { deleted = true });
        }

        /// <summary>
        /// Retrieves a selection of recipes for a specific meal type, excluding recipes used within a specified number of weeks.
        /// </summary>
        /// <param name="mealId">The ID of the meal type for which recipes are needed (e.g., Breakfast, Lunch, Dinner).</param>
        /// <param name="excludeWeeks">Number of weeks to look back for used recipes to avoid repetition.</param>
        /// <returns>A selection of up to 3 recipes suitable for the specified meal type that haven't been used recently.</returns>
        /// <response code="200">Returns a selection of recipes for the specified meal type.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="404">Not Found - meal type with the specified ID does not exist.</response>
        /// <remarks>
        /// Requires user authentication. This endpoint is designed for meal planning to help users avoid recipe repetition.
        /// The system looks back the specified number of weeks in the user's meal history and excludes any recipes
        /// that have been used during that period. Returns up to 3 recipes (as defined by CHOOSE_SIZE constant)
        /// that are associated with the specified meal type and haven't been used recently.
        /// This helps with meal variety and planning.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated.</exception>
        [HttpGet("api/recipes/choices")]
        public IActionResult GetRecipeToChoose([FromQuery] int mealId, [FromQuery] int excludeWeeks)
        {
                int? userId = Authentication.GetUserIdFromHeader(Request);
                if(userId==null)
                {
                    throw new UnauthorizedAccessException("You are not authorized to do this");
                }
                RecipeRecord[] items = _service.GetRecipeToChoose(mealId, userId.Value, excludeWeeks, CHOOSE_SIZE);
                return Ok(items);
        }
    }
}

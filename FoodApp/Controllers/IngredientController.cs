using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    /// <summary>
    /// Controller for managing ingredients, which are used to link foods and recipes together.
    /// Ingredients represent the relationship between a food item, a unit of measurement, and a recipe.
    /// </summary>
    [ApiController]
    public class IngredientController : Controller
    {
        private readonly IIngredientService _service;
        private readonly IAuthService _auth;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientController"/> class.
        /// </summary>
        /// <param name="service">The ingredient service for data operations.</param>
        /// <param name="authService">The authentication service for permission checks.</param>
        public IngredientController(IIngredientService service, IAuthService authService)
        {
            _service = service;
            _auth = authService;
        }

        /// <summary>
        /// Retrieves a list of ingredients with optional filtering and pagination.
        /// </summary>
        /// <param name="recipeId">Optional recipe ID to filter ingredients by recipe.</param>
        /// <param name="foodId">Optional food ID to filter ingredients by food item.</param>
        /// <param name="page">Page number for pagination (default: 1).</param>
        /// <param name="perPage">Number of items per page (default: 25, max: 100).</param>
        /// <returns>A list of ingredients matching the search criteria with associated food and unit details.</returns>
        /// <response code="200">Returns the list of matching ingredients.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <remarks>
        /// Requires user authentication. Supports filtering by either recipeId or foodId, but not both simultaneously.
        /// Each ingredient includes detailed information about the associated food item and measurement unit.
        /// Results are sorted by ingredient ID in ascending order.
        /// </remarks>
        [HttpGet("api/ingredients")]
        public IActionResult GetIngredients([FromQuery] Guid? recipeId = null, [FromQuery] Guid? foodId = null, [FromQuery] int page = 1, [FromQuery]int perPage = 25)
        {
                Authentication.ValidateToken(Request);
                Ingredient[] ingredients = _service.GetIngredients(recipeId, foodId,page, perPage).ToArray();
                return Ok(ingredients);
        }

        /// <summary>
        /// Retrieves a specific ingredient by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the ingredient.</param>
        /// <returns>The ingredient with associated food and unit details if found; otherwise returns 404 Not Found.</returns>
        /// <response code="200">Returns the requested ingredient with full details.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="404">Not Found - ingredient with the specified ID does not exist.</response>
        /// <remarks>
        /// Requires user authentication. Returns complete ingredient information including
        /// the associated food item and measurement unit details.
        /// </remarks>
        [HttpGet("api/ingredients/{id}")]
        public IActionResult GetIngredient(Guid id)
        {
                Authentication.ValidateToken(Request);
                var item = _service.GetIngredientById(id);
                if (item == null)
                {
                    return NotFound();
                }
                return Ok(item);
        }

        /// <summary>
        /// Creates a new ingredient record.
        /// </summary>
        /// <param name="request">The ingredient data to create. Must include valid foodId, unitId, unitAmount, and recipeId.</param>
        /// <param name="authorization">The authorization header containing the bearer token.</param>
        /// <returns>The created ingredient with its assigned ID.</returns>
        /// <response code="200">Returns the created ingredient.</response>
        /// <response code="400">Bad Request - invalid ingredient data or missing required fields.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. The ingredient links a specific food item to a recipe
        /// with a specified quantity and unit of measurement. All referenced IDs (foodId, unitId, recipeId)
        /// must exist in the database. The unitAmount must be a positive decimal value.
        /// </remarks>
        [HttpPost("api/ingredients")]
        public IActionResult PostIngredient([FromBody] IngredientCreateRequest request)
        {
                var userId = _auth.CheckPermissions(Request, [Roles.admin]);
                var created = _service.CreateIngredient(request);
                return Ok(created);
        }

        /// <summary>
        /// Updates an existing ingredient record.
        /// </summary>
        /// <param name="id">The ID of the ingredient to update (must match the ingredient's ID in the request body).</param>
        /// <param name="ingredient">The updated ingredient data. The ID in the body must match the route parameter.</param>
        /// <param name="authorization">The authorization header containing the bearer token.</param>
        /// <returns>The updated ingredient, or 400 Bad Request if no valid fields to update or ingredient not found.</returns>
        /// <response code="200">Returns the updated ingredient.</response>
        /// <response code="400">Bad Request - no valid fields provided for update or ingredient not found.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. The ingredient object must have its ID property set to match the route parameter.
        /// Only non-null properties will be updated. Returns 400 if no fields are provided for update.
        /// All referenced IDs must remain valid after the update.
        /// </remarks>
        [HttpPatch("api/ingredients/{id}")]
        public IActionResult PatchIngredient(Guid id, [FromBody] IngredientUpdateRequest ingredient)
        {
                var userId = _auth.CheckPermissions(Request, [Roles.admin]);
                var updated = _service.UpdateIngredient(id,ingredient);
                if (updated == null)
                {
                    return BadRequest(new { error = "No fields or not found" });
                }
                return Ok(updated);
        }

        /// <summary>
        /// Deletes an ingredient by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the ingredient to delete.</param>
        /// <param name="authorization">The authorization header containing the bearer token.</param>
        /// <returns>Success status of the deletion operation.</returns>
        /// <response code="200">Returns deletion status (true if successful, false if not found).</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. This operation permanently removes the ingredient from the database.
        /// This breaks the link between the food item and recipe. The associated food item and recipe remain in the system.
        /// </remarks>
        [HttpDelete("api/ingredients/{id}")]
        public IActionResult DeleteIngredient(Guid id)
        {
                var userId = _auth.CheckPermissions(Request, [Roles.admin]);
                _service.DeleteIngredient(id);
                return Ok(new { deleted = true });
        }
    }
}

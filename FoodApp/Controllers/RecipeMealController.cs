using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    /// <summary>
    /// API controller for managing recipe-meal associations. This controller handles the many-to-many relationship
    /// between recipes and meal types, allowing recipes to be associated with multiple meal types (e.g., a recipe
    /// could be suitable for both Lunch and Dinner).
    /// </summary>
    [ApiController]
    public class RecipeMealController : Controller
    {
        private readonly IRecipeMealService _service;
        private readonly IAuthService _auth;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeMealController"/> class.
        /// </summary>
        /// <param name="service">The recipe-meal service for data operations.</param>
        /// <param name="authService">The authentication service for permission checks.</param>
        public RecipeMealController(IRecipeMealService service, IAuthService authService)
        {
            _service = service;
            _auth = authService;
        }

        /// <summary>
        /// Retrieves recipe-meal associations with optional filtering.
        /// </summary>
        /// <param name="recipeId">Optional recipe ID to filter associations by recipe.</param>
        /// <param name="mealId">Optional meal ID to filter associations by meal type.</param>
        /// <returns>A list of recipe-meal associations matching the search criteria.</returns>
        /// <response code="200">Returns the list of matching recipe-meal associations.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <remarks>
        /// Requires user authentication. Supports filtering by either recipeId or mealId, but not both simultaneously.
        /// Each association includes the recipe ID, meal ID, and the full meal details.
        /// Results are sorted by recipe ID in ascending order.
        /// </remarks>
        [HttpGet("api/recipe_meals")]
        public IActionResult GetRecipeMeals([FromQuery] Guid? recipeId = null, [FromQuery] Guid? mealId = null)
        {
            Authentication.ValidateToken(Request);
            return Ok(_service.GetRecipeMeals(recipeId, mealId));
        }

        /// <summary>
        /// Retrieves a specific recipe-meal association by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the recipe-meal association.</param>
        /// <returns>The recipe-meal association if found; otherwise returns 404 Not Found.</returns>
        /// <response code="200">Returns the requested recipe-meal association.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="404">Not Found - recipe-meal association with the specified ID does not exist.</response>
        /// <remarks>
        /// Requires user authentication. Returns complete information about the association
        /// including the recipe ID, meal ID, and meal details.
        /// </remarks>
        [HttpGet("api/recipe_meals/{id}")]
        public IActionResult GetRecipeMeal(Guid id)
        {
            Authentication.ValidateToken(Request);
            var item = _service.GetRecipeMealById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        /// <summary>
        /// Creates a new recipe-meal association.
        /// </summary>
        /// <param name="request">The recipe-meal association data to create. Must include valid recipeId and mealId.</param>
        /// <returns>The created recipe-meal association with its assigned ID.</returns>
        /// <response code="200">Returns the created recipe-meal association.</response>
        /// <response code="400">Bad Request - invalid association data or missing required fields.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. Creates an association between a specific recipe and meal type.
        /// Both recipeId and mealId must reference existing records in the database.
        /// Duplicate associations (same recipe and meal combination) are not allowed.
        /// </remarks>
        [HttpPost("api/recipe_meals")]
        public IActionResult PostRecipeMeal([FromBody] CreateRecipeMealRequest request)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            var created = _service.CreateRecipeMeal(request);
            return Ok(created);
        }

        /// <summary>
        /// Deletes a recipe-meal association by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the recipe-meal association to delete.</param>
        /// <returns>Success status of the deletion operation.</returns>
        /// <response code="200">Returns deletion status (true if successful, false if not found).</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. This operation removes the association between a recipe and meal type.
        /// The recipe and meal records themselves remain in the database.
        /// This is a safe operation that only removes the linking record.
        /// </remarks>
        [HttpDelete("api/recipe_meals/{id}")]
        public IActionResult DeleteRecipeMeal(Guid id)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            _service.DeleteRecipeMeal(id);
            return Ok(new { deleted = true });
        }
    }
}

using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    /// <summary>
    /// API controller for managing meal types. Provides endpoints to create, read, update, and delete meal records.
    /// Meals represent categories or types of meals (e.g., Breakfast, Lunch, Dinner, Snack) that can be associated with recipes.
    /// </summary>
    [ApiController]
    public class MealController : Controller
    {
        private readonly IMealService _service;
        private readonly IAuthService _auth;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MealController"/> class.
        /// </summary>
        /// <param name="service">The meal service for data operations.</param>
        /// <param name="authService">The authentication service for permission checks.</param>
        public MealController(IMealService service, IAuthService authService)
        {
            _service = service;
            _auth = authService;
        }

        /// <summary>
        /// Retrieves a list of meals with optional filtering and pagination.
        /// </summary>
        /// <param name="name">Optional name to search for (partial match, case-insensitive).</param>
        /// <param name="page">Page number for pagination (default: 1).</param>
        /// <param name="perPage">Number of items per page (default: 25, max: 100).</param>
        /// <returns>A list of meals matching the search criteria.</returns>
        /// <response code="200">Returns the list of matching meals.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <remarks>
        /// Requires user authentication. Supports pagination with page and perPage parameters.
        /// If name is provided, meals containing that text in their name are returned.
        /// Results are sorted alphabetically by name.
        /// </remarks>
        [HttpGet("api/meals")]
        public IActionResult GetMeals([FromQuery] string name = "", [FromQuery] int page = 1, [FromQuery] int perPage = 25)
        {
                Authentication.ValidateToken(Request);
                var response = _service.GetMeals(name, null, page, perPage);
                Console.WriteLine(response.Length);
                return Ok(response);
        }

        /// <summary>
        /// Retrieves a specific meal by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the meal.</param>
        /// <returns>The meal if found; otherwise returns 404 Not Found.</returns>
        /// <response code="200">Returns the requested meal.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="404">Not Found - meal with the specified ID does not exist.</response>
        /// <remarks>
        /// Requires user authentication. Returns detailed information about a single meal.
        /// </remarks>
        [HttpGet("api/meals/{id}")]
        public IActionResult GetMeal(Guid id)
        {
            Authentication.ValidateToken(Request);
                var item = _service.GetMealById(id);
                if (item == null)
                {
                    return NotFound();
                }
                return Ok(item); 
        }

        /// <summary>
        /// Creates a new meal record.
        /// </summary>
        /// <param name="mealRequest">The meal data to create. Name is required and must be unique.</param>
        /// <returns>The created meal with its assigned ID.</returns>
        /// <response code="200">Returns the created meal.</response>
        /// <response code="400">Bad Request - invalid meal data or missing required fields.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. The meal name must be unique within the system.
        /// Meal names are trimmed of whitespace and validated for minimum length.
        /// Common meal types include Breakfast, Lunch, Dinner, Snack, but custom types are supported.
        /// </remarks>
        [HttpPost("api/meals")]
        public IActionResult PostMeal([FromBody] MealCreateRequest mealRequest)
        {
                _auth.CheckPermissions(Request, [Roles.admin]);
                var created = _service.CreateMeal(mealRequest);
                return Ok(created);
        }

        /// <summary>
        /// Updates an existing meal record.
        /// </summary>
        /// <param name="id">The ID of the meal to update.</param>
        /// <param name="request">Meal Update Request object</param>
        /// <returns>The updated meal, or 400 Bad Request if no valid fields to update or meal not found.</returns>
        /// <response code="200">Returns the updated meal.</response>
        /// <response code="400">Bad Request - no valid fields provided for update or meal not found.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. Only the name field can be updated.
        /// The updated name must be unique within the system.
        /// Returns 400 if no fields are provided for update.
        /// </remarks>
        [HttpPatch("api/meals/{id}")]
        public IActionResult PatchMeal(Guid id, [FromBody] MealUpdateRequest request)
        {
                var userId = _auth.CheckPermissions(Request, [Roles.admin]);
                var updated = _service.UpdateMeal(id, request);
                if (updated == null) return BadRequest(new { error = "No fields or not found" });
                return Ok(updated);
        }

        /// <summary>
        /// Deletes a meal by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the meal to delete.</param>
        /// <returns>Success status of the deletion operation.</returns>
        /// <response code="200">Returns deletion status (true if successful, false if not found).</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. This operation performs a cascading update:
        /// - The meal is permanently removed from the database
        /// - All recipes that had this meal association will have their meal references removed
        /// - This is a destructive operation that cannot be undone
        /// </remarks>
        [HttpDelete("api/meals/{id}")]
        public IActionResult DeleteMeal(Guid id)
        {
                var userId = _auth.CheckPermissions(Request, [Roles.admin]);
                _service.DeleteMeal(id);
                return Ok(new { deleted = true });
        }
    }
}

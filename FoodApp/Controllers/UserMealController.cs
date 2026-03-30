using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    /// <summary>
    /// API controller for managing user meal records. Provides endpoints to create, read, and delete
    /// meal records that track what recipes users have planned for specific meals and dates.
    /// This controller handles meal planning and scheduling functionality for authenticated users.
    /// </summary>
    [ApiController]
    public class UserMealController : Controller
    {
        private readonly IUserMealService _service;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UserMealController"/> class.
        /// </summary>
        /// <param name="service">The user meal service for data operations.</param>
        public UserMealController(IUserMealService service) { _service = service; }

        /// <summary>
        /// Retrieves the current user's meal records with optional date filtering.
        /// </summary>
        /// <param name="startDate">Optional start date to filter meal records (inclusive).</param>
        /// <param name="endDate">Optional end date to filter meal records (inclusive).</param>
        /// <returns>A list of user meal records with associated recipe and meal details.</returns>
        /// <response code="200">Returns the list of matching user meal records.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <remarks>
        /// Requires user authentication. Returns all meal records for the authenticated user within the specified date range.
        /// If no date range is provided, returns all meal records for the user.
        /// Each record includes the recipe details, meal type, and scheduled date.
        /// This endpoint is useful for meal planning views and scheduling applications.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated.</exception>
        [HttpGet("api/user_meals")]
        
        public IActionResult GetUserMeals([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var userId = Authentication.GetUserIdFromHeader(Request);
            if (userId == null)
            {
                throw new UnauthorizedAccessException("You don't have permission to do this");
            }
            return Ok(_service.GetUserMeals(userId.Value, startDate, endDate));
        }

        /// <summary>
        /// Retrieves a specific user meal record by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the user meal record.</param>
        /// <returns>The user meal record with associated recipe and meal details if found; otherwise returns 404 Not Found.</returns>
        /// <response code="200">Returns the requested user meal record.</response>
        /// <response code="404">Not Found - user meal record with the specified ID does not exist.</response>
        /// <remarks>
        /// Requires user authentication. Returns detailed information about a specific meal record
        /// including the associated recipe, meal type, and scheduled date.
        /// Note that users can only access their own meal records.
        /// </remarks>
        [HttpGet("api/user_meals/{id}")]
        public IActionResult GetUserMeal(int id)
        {
                var item = _service.GetUserMealById(id); 
                if (item == null) return NotFound(); 
                return Ok(item);
        }

        /// <summary>
        /// Creates a new user meal record.
        /// </summary>
        /// <param name="request">The meal record data to create. Must include recipeId, mealId, and mealDate.</param>
        /// <returns>The created user meal record with its assigned ID.</returns>
        /// <response code="200">Returns the created user meal record.</response>
        /// <response code="400">Bad Request - invalid meal record data or missing required fields.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <remarks>
        /// Requires user authentication. Creates a new meal record for the authenticated user.
        /// The userId is automatically set from the authentication token.
        /// The recipeId must reference an existing recipe, and mealId must reference an existing meal type.
        /// The mealDate specifies when the user plans to prepare this meal.
        /// This endpoint is used for meal planning and scheduling functionality.
        /// </remarks>
        [HttpPost("api/user_meals")]
        public IActionResult PostUserMeal([FromBody] UserMealCreateRequest request)
        {
                var userId = Authentication.GetUserIdFromHeader(Request);
                if(userId==null)
                {
                    return Unauthorized(new { error = "You don't have permission to do this" });
                }
                var created = _service.CreateUserMeal(request, userId.Value); 
                return Ok(created);
        }

        /// <summary>
        /// Deletes a user meal record by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the user meal record to delete.</param>
        /// <returns>Success status of the deletion operation.</returns>
        /// <response code="200">Returns deletion status (true if successful, false if not found).</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <remarks>
        /// Requires user authentication. This operation permanently removes the meal record from the user's schedule.
        /// The associated recipe and meal type records remain in the system.
        /// This is a safe operation that only removes the user's specific meal plan record.
        /// Users can only delete their own meal records.
        /// </remarks>
        [HttpDelete("api/user_meals/{id}")]
        public IActionResult DeleteUserMeal(int id)
        {
            _service.DeleteUserMeal(id); return Ok(new { deleted = true });
        }
    }
}

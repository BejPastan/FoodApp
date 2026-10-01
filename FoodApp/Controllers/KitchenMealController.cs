using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Services.Interfaces;
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
    [Route("api/kitchen/")]
    public class KitchenMealController : Controller
    {
        private readonly IKitchenMealService _service;
        private readonly IKitchenUsersService _kitchenUsers;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UserMealController"/> class.
        /// </summary>
        /// <param name="service">The user meal service for data operations.</param>
        public KitchenMealController(IKitchenMealService service, IKitchenUsersService kitchenUsers) 
        {
            _kitchenUsers = kitchenUsers;
            _service = service; 
        }

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
        [HttpGet("{kitchen_id}/meals")]
        public IActionResult GetKitchenMeals([FromRoute] Guid kitchen_id, [FromQuery] DateOnly? startDate = null, [FromQuery] DateOnly? endDate = null)
        {
            var userId = Authentication.GetUserIdFromHeader(Request);
            if (userId == null)
            {
                throw new UnauthorizedAccessException("You don't have permission to do this");
            }
            if (!_kitchenUsers.CheckPermission(userId.Value, kitchen_id, KitchenRoles.inspector))
            {
                throw new UnauthorizedAccessException("You don't have access to do this");
            }
            return Ok(_service.GetKitchenMeals(kitchen_id, startDate, endDate));
        }

        /// <summary>
        /// Retrieves a specific user meal record by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the kitchen meal record.</param>
        /// <returns>The user meal record with associated recipe and meal details if found; otherwise returns 404 Not Found.</returns>
        /// <response code="200">Returns the requested user meal record.</response>
        /// <response code="404">Not Found - user meal record with the specified ID does not exist.</response>
        /// <remarks>
        /// Requires user authentication. Returns detailed information about a specific meal record
        /// including the associated recipe, meal type, and scheduled date.
        /// Note that users can only access their own meal records.
        /// </remarks>
        [HttpGet("meals/{id}")]
        public IActionResult GetKitchenMeal(Guid id)
        {
            
            Guid? userId = Authentication.GetUserIdFromHeader(Request);
            Guid kitchenId = _service.GetKitchenByMealId(id).Id;
            if (userId == null)
            {
                throw new UnauthorizedAccessException("You are not authorized to do this");
            }
            if (!_kitchenUsers.CheckPermission(userId.Value, kitchenId, KitchenRoles.inspector))
            {
                throw new UnauthorizedAccessException("You don't have access to do this");
            }
            var item = _service.GetKitchenMealById(id); 
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
        [HttpPost("{kitchen_id}/meals")]
        public IActionResult PostUserMeal([FromRoute] Guid kitchen_id, [FromBody] KitchenMealCreateRequest request)
        {
            Guid? userId = Authentication.GetUserIdFromHeader(Request);
            if (userId == null)
            {
                throw new UnauthorizedAccessException("You are not authorized to do this");
            }
            if (!_kitchenUsers.CheckPermission(userId.Value, kitchen_id, KitchenRoles.editor))
            {
                throw new UnauthorizedAccessException("You don't have access to do this");
            }
            var created = _service.CreateKitchenMeal(kitchen_id, request.recipeId, request.mealId, request.mealDate);
                return Ok(created);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">Returns the updated kitchen meal record.</response>
        [HttpPatch("{kitchen_id}/meal")]
        public IActionResult ChangeMeal([FromRoute] Guid kitchen_id, [FromBody] KitchenMealUpdateRequest request)
        {

            Guid? userId = Authentication.GetUserIdFromHeader(Request);
            if (userId == null)
            {
                throw new UnauthorizedAccessException("You are not authorized to do this");
            }
            if (!_kitchenUsers.CheckPermission(userId.Value, kitchen_id, KitchenRoles.editor))
            {
                throw new UnauthorizedAccessException("You don't have access to do this");
            }
            var created = _service.UpdateKitchenMeal(request.kitchenMealId, request.recipeId);
            return Ok(created);
        }

        /// <summary>
        /// Deletes a user meal record by its ID.
        /// </summary>
        /// <param name="kitchen_id">The unique identifier of the kitchen.</param>
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
        [HttpDelete("{kitchen_id}/meals/{id}")]
        public IActionResult DeleteUserMeal([FromRoute] Guid kitchen_id, [FromRoute] Guid id)
        {
            Guid? userId = Authentication.GetUserIdFromHeader(Request);
            if (userId == null)
            {
                throw new UnauthorizedAccessException("You are not authorized to do this");
            }
            if (!_kitchenUsers.CheckPermission(userId.Value, kitchen_id, KitchenRoles.editor))
            {
                throw new UnauthorizedAccessException("You don't have access to do this");
            }
            _service.DeleteKitchenMeal(id); return Ok(new { deleted = true });
        }


        /// <summary>
        /// Retrieves a selection of recipes for a specific meal type, excluding recipes used within a specified number of weeks.
        /// </summary>
        /// <param name="kitchen_id">The ID of the kitchen.</param>
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
        [HttpGet("{kitchen_id}/choices")]
        public IActionResult GetRecipeToChoose([FromRoute] Guid kitchen_id, [FromQuery] Guid mealId, [FromQuery] int excludeWeeks)
        {
            Guid? userId = Authentication.GetUserIdFromHeader(Request);
            if (userId == null)
            {
                throw new UnauthorizedAccessException("You are not authorized to do this");
            }
            if (!_kitchenUsers.CheckPermission(userId.Value, kitchen_id, KitchenRoles.editor))
            {
                throw new UnauthorizedAccessException("You don't have access to do this");
            }
            var result = _service.GetRecipeToChoose(mealId, kitchen_id, excludeWeeks, 3);
            return Ok(result);
        }
    }
}

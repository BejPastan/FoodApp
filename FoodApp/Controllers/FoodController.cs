using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    /// <summary>
    /// API controller for managing food items, including searching, retrieving, creating, updating, and deleting food records.
    /// </summary>
    [ApiController]
    public class FoodController : Controller
    {
        private readonly IFoodService _service;
        private readonly IAuthService _auth;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FoodController"/> class.
        /// </summary>
        /// <param name="service">The food service for data operations.</param>
        /// <param name="authService">The authentication service for permission checks.</param>
        public FoodController(IFoodService service, IAuthService authService)
        {
            _service = service;
            _auth = authService;
        }

        /// <summary>
        /// Searches for food items based on optional filters.
        /// </summary>
        /// <param name="typeId">Optional food type ID to filter by.</param>
        /// <param name="name">Optional name to search for (partial match).</param>
        /// <param name="page">Page number for pagination (default: 1).</param>
        /// <param name="perPage">Number of items per page (default: 10, max: 100).</param>
        /// <returns>A list of food items matching the search criteria.</returns>
        /// <response code="200">Returns the list of matching food items.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <remarks>
        /// Requires user authentication. Supports pagination with page and perPage parameters.
        /// If typeId is provided, only foods of that type are returned.
        /// If name is provided, foods containing that text in their name are returned.
        /// </remarks>
        [HttpGet("api/food")]
        public IActionResult SearchFood(
            [FromQuery] int? typeId,
            [FromQuery] string name = "",
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10
        )
        {
                Authentication.ValidateToken(Request);
                var foods = _service.GetFoods(typeId, name, page, perPage);
                return Ok(foods);
        }

        /// <summary>
        /// Retrieves a specific food item by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the food item.</param>
        /// <returns>The food item if found; otherwise returns 404 Not Found.</returns>
        /// <response code="200">Returns the requested food item.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="404">Not Found - food item with the specified ID does not exist.</response>
        /// <remarks>
        /// Requires user authentication. Returns detailed information about a single food item.
        /// </remarks>
        [HttpGet("api/food/{id}")]
        public IActionResult GetFood(int id)
        {
                Authentication.ValidateToken(Request);
                var food = _service.GetFoodById(id);
                if (food == null) return NotFound();
                return Ok(food);
        }

        /// <summary>
        /// Creates a new food record.
        /// </summary>
        /// <param name="request">The food data to create. Must include either foodTypeId or foodType.name.</param>
        /// <returns>The created food item with its assigned ID.</returns>
        /// <response code="200">Returns the created food item.</response>
        /// <response code="400">Bad Request - invalid food data or missing required fields.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. Either foodTypeId must be provided, or foodType.name must be specified
        /// to create a new food type. The system will automatically handle food type creation if needed.
        /// </remarks>
        /// <exception cref="Exception">Thrown when neither foodTypeId nor foodType.name is provided.</exception>
        [HttpPost("api/food")]
        public IActionResult PostFood([FromBody] CreteFoodRequest request)
        {
                var userId = _auth.CheckPermissions(Request,  [Roles.admin]);
                if(request.foodTypeId ==0 && (request.foodType == null || string.IsNullOrEmpty(request.foodType.name)))
                {
                    throw new Exception("food type id or food type name required");
                }
                var created = _service.CreateFood(request);
                return Ok(created);
        }

        /// <summary>
        /// Updates an existing food record.
        /// </summary>
        /// <param name="id">The ID of the food item to update.</param>
        /// <param name="food">The updated food data. The ID in the body must match the route parameter.</param>
        /// <returns>The updated food item, or 400 Bad Request if no valid fields to update or item not found.</returns>
        /// <response code="200">Returns the updated food item.</response>
        /// <response code="400">Bad Request - no valid fields provided for update or food item not found.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. The food object must have its ID property set to match the route parameter.
        /// Only non-null properties will be updated. Returns 400 if no fields are provided for update.
        /// </remarks>
        [HttpPatch("api/food/{id}")]
        public IActionResult PatchFood(
            int id,
            [FromBody] UpdateFoodRequest food
        )
        {
                var userId = _auth.CheckPermissions(Request,  [Roles.admin]);
                var updated = _service.UpdateFood(id, food);
                if (updated == null) return BadRequest(new { error = "No fields or not found" });
                return Ok(updated);
        }

        /// <summary>
        /// Deletes a food item by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the food item to delete.</param>
        /// <returns>Success status of the deletion operation.</returns>
        /// <response code="200">Returns deletion status (true if successful, false if not found).</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. This operation is soft delete - the food item is marked as deleted
        /// rather than being permanently removed from the database.
        /// </remarks>
        [HttpDelete("api/food/{id}")]
        public IActionResult DeleteFood(int id)
        {
                var userId = _auth.CheckPermissions(Request,  [Roles.admin]);
                var ok = _service.DeleteFood(id);
                return Ok(new { deleted = ok });
        }
    }
}

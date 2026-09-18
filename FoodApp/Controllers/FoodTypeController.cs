using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    /// <summary>
    /// API controller for managing food types. Provides endpoints to create, read, update, and delete food types.
    /// </summary>
    [ApiController]
    public class FoodTypeController(IFoodTypeService service, IAuthService authService) : Controller
    {
        private readonly IFoodTypeService _service = service;
        private readonly IAuthService _auth = authService;

        /// <summary>
        /// Retrieves a list of food types with optional filtering and pagination.
        /// </summary>
        /// <param name="name">Optional name to search for (partial match, case-insensitive).</param>
        /// <param name="page">Page number for pagination (default: 1).</param>
        /// <param name="perPage">Number of items per page (default: 25, max: 100).</param>
        /// <returns>A list of food types matching the search criteria.</returns>
        /// <response code="200">Returns the list of matching food types.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <remarks>
        /// Requires user authentication. Supports pagination with page and perPage parameters.
        /// If name is provided, food types containing that text in their name are returned.
        /// Results are sorted alphabetically by name.
        /// </remarks>
        [HttpGet("api/food_type")]
        public IActionResult GetFoodTypes([FromQuery] string name = "", [FromQuery] int page = 1, [FromQuery] int perPage = 25)
        {
                Authentication.ValidateToken(Request);
                var foodTypes = _service.GetFoodTypes(name, page, perPage);
                return Ok(foodTypes);
        }

        /// <summary>
        /// Retrieves a specific food type by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the food type.</param>
        /// <returns>The food type if found; otherwise returns 404 Not Found.</returns>
        /// <response code="200">Returns the requested food type.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="404">Not Found - food type with the specified ID does not exist.</response>
        /// <remarks>
        /// Requires user authentication. Returns detailed information about a single food type.
        /// </remarks>
        [HttpGet("api/food_type/{id}")]
        public IActionResult GetFoodType(Guid id)
        {
                Authentication.ValidateToken(Request);
                var foodType = _service.GetFoodTypeById(id);
                if (foodType == null) return NotFound();
                return Ok(foodType);
        }

        /// <summary>
        /// Creates a new food type.
        /// </summary>
        /// <param name="request">The food type data to create. Name is required and must be unique.</param>
        /// <returns>The created food type with its assigned ID.</returns>
        /// <response code="200">Returns the created food type.</response>
        /// <response code="400">Bad Request - invalid food type data or missing required fields.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. The food type name must be unique within the system.
        /// Food type names are trimmed of whitespace and validated for minimum length.
        /// </remarks>
        [HttpPost("api/food_type")]
        public IActionResult PostFoodType([FromBody] FoodTypeCreateRequest request)
        {
                _auth.CheckPermissions(Request, [Roles.admin]);
                if (string.IsNullOrWhiteSpace(request.name))
                {
                    return BadRequest(new { error = "Name is required." });
                }
                var created = _service.CreateFoodType(request);
                return Ok(created);
        }
        

        /// <summary>
        /// Updates an existing food type.
        /// </summary>
        /// <param name="id">The ID of the food type to update.</param>
        /// <param name="request">The updated food type data. Only provided fields will be updated.</param>
        /// <returns>The updated food type, or 404 Not Found if the food type does not exist.</returns>
        /// <response code="200">Returns the updated food type.</response>
        /// <response code="400">Bad Request - no valid fields provided for update.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <response code="404">Not Found - food type with the specified ID does not exist.</response>
        /// <remarks>
        /// Requires admin role authentication. Only the name field can be updated.
        /// The updated name must be unique within the system.
        /// Returns 400 if no fields are provided for update.
        /// </remarks>
        [HttpPatch("api/food_type/{id}")]
        public IActionResult PatchFoodType(Guid id, [FromBody] FoodTypeUpdateRequest request)
        {
                _auth.CheckPermissions(Request, [Roles.admin]);
                if (request.name == null) return BadRequest(new { error = "No fields provided to update." });
                var updated = _service.UpdateFoodType(id, request);
                if (updated == null) return NotFound();
                return Ok(updated);
        }

        /// <summary>
        /// Deletes a food type by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the food type to delete.</param>
        /// <returns>Success status of the deletion operation.</returns>
        /// <response code="200">Returns deletion status (true if successful, false if not found).</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. This operation performs a cascading update:
        /// - The food type is permanently removed from the database
        /// - All foods that had this type will have their type_id set to null
        /// - This is a destructive operation that cannot be undone
        /// </remarks>
        [HttpDelete("api/food_type/{id}")]
        public IActionResult DeleteFoodType(Guid id)
        {
                _auth.CheckPermissions(Request, [Roles.admin]);
                var ok = _service.DeleteFoodType(id);
                return Ok(new { success = ok });
        }
    }
}

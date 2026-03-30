using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    /// <summary>
    /// API controller for managing recipe steps. Provides endpoints to create, read, update, and delete
    /// cooking instructions and preparation steps for recipes. Steps represent the sequential instructions
    /// that users follow to prepare a recipe.
    /// </summary>
    [ApiController]
    public class StepController : Controller
    {
        private readonly IStepService _service;
        private readonly IAuthService _auth;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="StepController"/> class.
        /// </summary>
        /// <param name="service">The step service for data operations.</param>
        /// <param name="authService">The authentication service for permission checks.</param>
        public StepController(IStepService service, IAuthService authService)
        {
            _service = service;
            _auth = authService;
        }

        /// <summary>
        /// Retrieves a list of steps with optional filtering and pagination.
        /// </summary>
        /// <param name="recipeId">Optional recipe ID to filter steps by recipe.</param>
        /// <param name="page">Page number for pagination (default: 1).</param>
        /// <param name="perPage">Number of items per page (default: 25, max: 100).</param>
        /// <returns>A list of steps matching the search criteria, sorted by stepNumber.</returns>
        /// <response code="200">Returns the list of matching steps.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <remarks>
        /// Requires user authentication. Supports filtering by recipeId to get all steps for a specific recipe.
        /// Steps are returned in sequential order by their stepNumber property.
        /// Each step includes its instruction text and the recipe it belongs to.
        /// </remarks>
        [HttpGet("api/steps")]
        public IActionResult GetSteps([FromQuery] int? recipeId = null, [FromQuery] int page = 1, [FromQuery] int perPage = 25)
        {
            Authentication.ValidateToken(Request);
            return Ok(_service.GetSteps(recipeId, page, perPage));
        }

        /// <summary>
        /// Retrieves a specific step by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the step.</param>
        /// <returns>The step with its details if found; otherwise returns 404 Not Found.</returns>
        /// <response code="200">Returns the requested step.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="404">Not Found - step with the specified ID does not exist.</response>
        /// <remarks>
        /// Requires user authentication. Returns detailed information about a single step
        /// including its instruction text, step number, and associated recipe.
        /// </remarks>
        [HttpGet("api/steps/{id}")]
        public IActionResult GetStep(int id)
        {
            Authentication.ValidateToken(Request);
            var item = _service.GetStepById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        /// <summary>
        /// Creates a new step for a recipe.
        /// </summary>
        /// <param name="request">The step data to create. Must include recipeId, instruction text, and stepNumber.</param>
        /// <returns>The created step with its assigned ID.</returns>
        /// <response code="200">Returns the created step.</response>
        /// <response code="400">Bad Request - invalid step data or missing required fields.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. Creates a new instruction step for a specific recipe.
        /// The recipeId must reference an existing recipe. The stepNumber should be unique within the recipe
        /// and represents the order in which this step should be performed. The instruction text should be
        /// clear and detailed enough for users to follow.
        /// </remarks>
        [HttpPost("api/steps")]
        public IActionResult PostStep([FromBody] StepCreateRequest request)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            var created = _service.CreateStep(request);
            return Ok(created);
        }

        /// <summary>
        /// Updates an existing step.
        /// </summary>
        /// <param name="id">The ID of the step to update (must match the step's ID in the request body).</param>
        /// <param name="stepRequest">The updated step data. The ID in the body must match the route parameter.</param>
        /// <returns>The updated step, or 400 Bad Request if no valid fields to update or step not found.</returns>
        /// <response code="200">Returns the updated step.</response>
        /// <response code="400">Bad Request - no valid fields provided for update or step not found.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. The step object must have its ID property set to match the route parameter.
        /// The instruction text, step number, and recipe association can be updated.
        /// Returns 400 if no fields are provided for update.
        /// All referenced IDs must remain valid after the update.
        /// </remarks>
        [HttpPatch("api/steps/{id}")]
        public IActionResult PatchStep(int id, [FromBody] StepUpdateRequest stepRequest)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            var updated = _service.UpdateStep(id, stepRequest);
            if (updated == null)
            {
                return BadRequest(new { error = "No fields or not found" });
            }
            return Ok(updated);
        }

        /// <summary>
        /// Deletes a step by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the step to delete.</param>
        /// <returns>Success status of the deletion operation.</returns>
        /// <response code="200">Returns deletion status (true if successful, false if not found).</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. This operation permanently removes the step from the database.
        /// The recipe itself remains in the system, but this specific instruction is lost.
        /// Consider updating the step instead of deleting it if you want to preserve the recipe structure.
        /// </remarks>
        [HttpDelete("api/steps/{id}")]
        public IActionResult DeleteStep(int id)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            _service.DeleteStep(id);
            return Ok(new { deleted = true });
        }
    }
}

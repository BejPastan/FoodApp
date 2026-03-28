using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    /// <summary>
    /// API controller for managing tags. Provides endpoints to create, read, update, and delete tags
    /// that can be used to categorize and organize recipes. Tags help users filter and discover recipes
    /// based on dietary preferences, cooking methods, cuisine types, or other custom categories.
    /// </summary>
    [ApiController]
    public class TagController : Controller
    {
        private readonly ITagService _service;
        private readonly IAuthService _auth;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TagController"/> class.
        /// </summary>
        /// <param name="service">The tag service for data operations.</param>
        /// <param name="authService">The authentication service for permission checks.</param>
        public TagController(ITagService service, IAuthService authService)
        {
            _service = service;
            _auth = authService;
        }

        /// <summary>
        /// Retrieves a list of tags with optional filtering.
        /// </summary>
        /// <param name="name">Optional name to search for (partial match, case-insensitive).</param>
        /// <returns>A list of tags matching the search criteria.</returns>
        /// <response code="200">Returns the list of matching tags.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <remarks>
        /// Requires user authentication. If name is provided, tags containing that text in their name are returned.
        /// Results are sorted alphabetically by name. Tags are used to categorize recipes for easier discovery.
        /// Common tag examples include "vegetarian", "gluten-free", "quick-meal", "italian", etc.
        /// </remarks>
        [HttpGet("api/tags")]
        public IActionResult GetTags([FromQuery] string name = "")
        {
            Authentication.ValidateToken(Request);
            return Ok(_service.GetTags(name));
        }

        /// <summary>
        /// Retrieves a specific tag by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the tag.</parameter>
        /// <returns>The tag if found; otherwise returns 404 Not Found.</returns>
        /// <response code="200">Returns the requested tag.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="404">Not Found - tag with the specified ID does not exist.</response>
        /// <remarks>
        /// Requires user authentication. Returns detailed information about a single tag.
        /// Tags are used to categorize recipes and help users filter recipes by dietary preferences,
        /// cooking methods, cuisine types, or other custom categories.
        /// </remarks>
        [HttpGet("api/tags/{id}")]
        public IActionResult GetTag(int id)
        {
            Authentication.ValidateToken(Request);
            var item = _service.GetTagById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        /// <summary>
        /// Creates a new tag.
        /// </summary>
        /// <param name="tagRequest">The tag data to create. Name is required and must be unique.</param>
        /// <returns>The created tag with its assigned ID.</returns>
        /// <response code="200">Returns the created tag.</response>
        /// <response code="400">Bad Request - invalid tag data or missing required fields.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. The tag name must be unique within the system.
        /// Tag names are trimmed of whitespace and validated for minimum length.
        /// Tags help organize recipes by categories like dietary restrictions, cooking methods,
        /// cuisine types, or any other meaningful classification system.
        /// </remarks>
        [HttpPost("api/tags")]
        public IActionResult PostTag([FromBody] Tag tagRequest)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            var created = _service.CreateTag(tagRequest);
            return Ok(created);
        }

        /// <summary>
        /// Updates an existing tag.
        /// </summary>
        /// <param name="id">The ID of the tag to update (must match the tag's ID in the request body).</param>
        /// <param name="request">The updated tag data. The ID in the body must match the route parameter.</param>
        /// <param name="authorization">The authorization header containing the bearer token.</param>
        /// <returns>The updated tag, or 400 Bad Request if no valid fields to update or tag not found.</returns>
        /// <response code="200">Returns the updated tag.</response>
        /// <response code="400">Bad Request - no valid fields provided for update or tag not found.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. The tag object must have its ID property set to match the route parameter.
        /// The updated name must be unique within the system. Returns 400 if no fields are provided for update.
        /// </remarks>
        [HttpPatch("api/tags/{id}")]
        public IActionResult PatchTag(int id, [FromBody] Tag request, [FromHeader(Name = "Authorization")] string authorization)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            var updated = _service.UpdateTag(request);
            if (updated == null)
            {
                return BadRequest(new { error = "No fields or not found" });
            }
            return Ok(updated);
        }

        /// <summary>
        /// Deletes a tag by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the tag to delete.</param>
        /// <param name="authorization">The authorization header containing the bearer token.</param>
        /// <returns>Success status of the deletion operation.</returns>
        /// <response code="200">Returns deletion status (true if successful, false if not found).</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. This operation performs a cascading update:
        /// - The tag is permanently removed from the database
        /// - All recipe-tag associations for this tag are also removed
        /// - Recipes themselves remain in the system, they just lose this tag classification
        /// This is a safe operation that preserves recipe data while removing the categorization.
        /// </remarks>
        [HttpDelete("api/tags/{id}")]
        public IActionResult DeleteTag(int id, [FromHeader(Name = "Authorization")] string authorization)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            _service.DeleteTag(id);
            return Ok(new { deleted = true });
        }
    }
}

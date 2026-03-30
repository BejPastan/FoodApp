using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    /// <summary>
    /// API controller for managing recipe-tag associations. This controller handles the many-to-many relationship
    /// between recipes and tags, allowing recipes to be categorized with multiple tags for better organization
    /// and searchability (e.g., "vegetarian", "gluten-free", "quick-meal").
    /// </summary>
    [ApiController]
    public class RecipeTagController : Controller
    {
        private readonly IRecipeTagService _service;
        private readonly IAuthService _auth;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeTagController"/> class.
        /// </summary>
        /// <param name="service">The recipe-tag service for data operations.</param>
        /// <param name="authService">The authentication service for permission checks.</param>
        public RecipeTagController(IRecipeTagService service, IAuthService authService)
        {
            _service = service;
            _auth = authService;
        }

        /// <summary>
        /// Retrieves recipe-tag associations with optional filtering.
        /// </summary>
        /// <param name="recipeId">Optional recipe ID to filter associations by recipe.</param>
        /// <param name="tagId">Optional tag ID to filter associations by tag.</param>
        /// <returns>A list of recipe-tag associations matching the search criteria.</returns>
        /// <response code="200">Returns the list of matching recipe-tag associations.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <remarks>
        /// Requires user authentication. Supports filtering by either recipeId or tagId, but not both simultaneously.
        /// Each association includes the recipe ID, tag ID, and the full tag details.
        /// Results are sorted by recipe ID in ascending order.
        /// </remarks>
        [HttpGet("api/recipe_tags")]
        public IActionResult GetRecipeTags([FromQuery] int? recipeId = null, [FromQuery] int? tagId = null)
        {
            Authentication.ValidateToken(Request);
            return Ok(_service.GetRecipeTags(recipeId, tagId));
        }

        /// <summary>
        /// Creates a new recipe-tag association.
        /// </summary>
        /// <param name="request">The recipe-tag association data to create. Must include valid recipeId and tagId.</param>
        /// <returns>The created recipe-tag association with its assigned ID.</returns>
        /// <response code="200">Returns the created recipe-tag association.</response>
        /// <response code="400">Bad Request - invalid association data or missing required fields.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. Creates an association between a specific recipe and tag.
        /// Both recipeId and tagId must reference existing records in the database.
        /// Duplicate associations (same recipe and tag combination) are not allowed.
        /// </remarks>
        [HttpPost("api/recipe_tags")]
        public IActionResult PostRecipeTag([FromBody] RecipeTagCreateRequest request)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            var created = _service.CreateRecipeTag(request);
            return Ok(created);
        }

        /// <summary>
        /// Deletes a recipe-tag association by recipe and tag IDs.
        /// </summary>
        /// <param name="recipeId">The unique identifier of the recipe.</param>
        /// <param name="tagId">The unique identifier of the tag.</param>
        /// <returns>Success status of the deletion operation.</returns>
        /// <response code="200">Returns deletion status (true if successful, false if not found).</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. This operation removes the association between a recipe and tag.
        /// The recipe and tag records themselves remain in the database.
        /// This is a safe operation that only removes the linking record.
        /// </remarks>
        [HttpDelete("api/recipe_tags")]
        public IActionResult DeleteRecipeTag([FromQuery] int recipeId, [FromQuery] int tagId)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            _service.DeleteRecipeTag(recipeId, tagId);
            return Ok(new { deleted = true });
        }
    }
}
